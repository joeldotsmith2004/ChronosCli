using Microsoft.Identity.Client;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;
using Spectre.Console;

public static class SecureMsalTokenCacheFile
{
    private static readonly SemaphoreSlim FileLock = new SemaphoreSlim(1, 1);

    private static string GetCacheDirectory()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".chronos");

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
            SetUnixFilePermissions(dir, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }

        return dir;
    }

    private static string GetCachePath()
    {
        return Path.Combine(GetCacheDirectory(), "token.bin");
    }

    private static string GetSaltPath()
    {
        return Path.Combine(GetCacheDirectory(), ".salt");
    }

    public static void RegisterTokenCache(ITokenCache tokenCache)
    {
        var cachePath = GetCachePath();

        tokenCache.SetBeforeAccess(args =>
        {
            FileLock.Wait();
            try
            {
                if (File.Exists(cachePath))
                {
                    var encryptedBlob = File.ReadAllBytes(cachePath);
                    var decryptedBlob = DecryptData(encryptedBlob);
                    args.TokenCache.DeserializeMsalV3(decryptedBlob);
                }
            }
            finally { FileLock.Release(); }
        });

        tokenCache.SetAfterAccess(args =>
        {
            if (!args.HasStateChanged) return;

            FileLock.Wait();
            try
            {
                var blob = args.TokenCache.SerializeMsalV3();
                var encryptedBlob = EncryptData(blob);
                File.WriteAllBytes(cachePath, encryptedBlob);
                SetUnixFilePermissions(cachePath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            }
            finally { FileLock.Release(); }
        });
    }

    private static byte[] DeriveKey(string password, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(32);
    }

    private static byte[] GetOrCreateSalt()
    {
        var saltPath = GetSaltPath();

        if (File.Exists(saltPath))
        {
            return File.ReadAllBytes(saltPath);
        }

        var salt = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        File.WriteAllBytes(saltPath, salt);
        SetUnixFilePermissions(saltPath, UnixFileMode.UserRead | UnixFileMode.UserWrite);

        return salt;
    }

    private static string GetMachineKey()
    {
        var machineInfo = new StringBuilder();
        machineInfo.Append(Environment.UserName);
        machineInfo.Append(Environment.MachineName);
        machineInfo.Append(Environment.ProcessorCount);
        machineInfo.Append(Environment.OSVersion.Platform);
        try
        {
            var uptimeMs = Environment.TickCount64;
            var bootTime = DateTime.Now.AddMilliseconds(-uptimeMs);
            machineInfo.Append(bootTime.Date.ToString("yyyyMMdd"));
            machineInfo.Append(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var machineIdPaths = new[] { "/etc/machine-id", "/var/lib/dbus/machine-id" };
                foreach (var path in machineIdPaths)
                {
                    try
                    {
                        if (File.Exists(path))
                        {
                            var machineId = File.ReadAllText(path).Trim();
                            if (!string.IsNullOrEmpty(machineId))
                            {
                                machineInfo.Append(machineId);
                                break;
                            }
                        }
                    }
                    catch { }
                }
            }
        }
        catch
        {
            machineInfo.Append("fallback-key");
        }

        var machineBytes = Encoding.UTF8.GetBytes(machineInfo.ToString());
        var hash = SHA256.HashData(machineBytes);
        return Convert.ToBase64String(hash);
    }

    private static byte[] EncryptData(byte[] data)
    {
        var salt = GetOrCreateSalt();
        var machineKey = GetMachineKey();
        var key = DeriveKey(machineKey, salt);

        try
        {
            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using var ms = new MemoryStream();
            ms.Write(aes.IV, 0, aes.IV.Length);

            using var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();

            return ms.ToArray();
        }
        finally
        {
            Array.Clear(key, 0, key.Length);
        }
    }

    private static byte[] DecryptData(byte[] encryptedData)
    {
        var salt = GetOrCreateSalt();
        var machineKey = GetMachineKey();
        var key = DeriveKey(machineKey, salt);

        try
        {
            using var aes = Aes.Create();
            aes.Key = key;

            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var ms = new MemoryStream(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using var result = new MemoryStream();

            cs.CopyTo(result);
            return result.ToArray();
        }
        finally
        {
            Array.Clear(key, 0, key.Length);
        }
    }

    private static void SetUnixFilePermissions(string path, UnixFileMode mode)
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                File.SetUnixFileMode(path, mode);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not set file permissions on {path}: {ex.Message}[/]");
        }
    }

    public static void ClearCache()
    {
        FileLock.Wait();
        try
        {
            var cachePath = GetCachePath();
            var saltPath = GetSaltPath();

            SecureDelete(cachePath);
            SecureDelete(saltPath);

            AnsiConsole.MarkupLine("[green]Cache securely cleared[/]");
        }
        finally { FileLock.Release(); }
    }

    private static void SecureDelete(string filePath)
    {
        if (!File.Exists(filePath)) return;

        try
        {
            var fileInfo = new FileInfo(filePath);
            var length = fileInfo.Length;

            using var rng = RandomNumberGenerator.Create();
            var buffer = new byte[4096];

            for (int pass = 0; pass < 3; pass++)
            {
                using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);
                for (long written = 0; written < length; written += buffer.Length)
                {
                    var toWrite = (int)Math.Min(buffer.Length, length - written);
                    rng.GetBytes(buffer, 0, toWrite);
                    fs.Write(buffer, 0, toWrite);
                }
                fs.Flush();
                fs.Close();
            }

            File.Delete(filePath);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not securely delete {filePath}: {ex.Message}[/]");
            try { File.Delete(filePath); } catch { }
        }
    }

    public static void ValidateSecurityContext()
    {
        if (Environment.UserName == "root")
        {
            AnsiConsole.MarkupLine("[yellow]Warning: Running as root user is not recommended for security[/]");
        }

        var cacheDir = GetCacheDirectory();
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var mode = File.GetUnixFileMode(cacheDir);
                var hasGroupAccess = (mode & (UnixFileMode.GroupRead | UnixFileMode.GroupWrite | UnixFileMode.GroupExecute)) != 0;
                var hasOtherAccess = (mode & (UnixFileMode.OtherRead | UnixFileMode.OtherWrite | UnixFileMode.OtherExecute)) != 0;

                if (hasGroupAccess || hasOtherAccess)
                {
                    AnsiConsole.MarkupLine($"[yellow]Warning: Cache directory {cacheDir} has potentially insecure permissions[/]");
                    SetUnixFilePermissions(cacheDir, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[yellow]Warning: Could not validate directory permissions: {ex.Message}[/]");
        }
    }
}
