using System.Text.Json;
using Backend.Core.Schemas;
using Spectre.Console;

public static class UserConfig
{
    static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".chronos");

    static readonly string ConfigPath = Path.Combine(ConfigDir, "user.json");

    public static async Task SaveAsync(UserBase info)
    {
        Directory.CreateDirectory(ConfigDir);
        var json = JsonSerializer.Serialize(info, ApiService.Instance.options);
        await File.WriteAllTextAsync(ConfigPath, json);
        Console.WriteLine($"Saved user config to path: {ConfigPath}");
    }

    public static async Task<UserBase?> LoadAsync()
    {
        if (!File.Exists(ConfigPath))
        {
            AnsiConsole.MarkupLine("No user config found. Try getting user info from API...");
            if (await TryGet() == 0)
            {
                AnsiConsole.MarkupLine($"[red]Error: Unable To Get User Info[/]");
                return null;
            }
        }
        var json = await File.ReadAllTextAsync(ConfigPath);
        return JsonSerializer.Deserialize<UserBase>(json, ApiService.Instance.options);
    }

    public static async Task<int> TryGet()
    {
        var res = await ApiService.Instance.GetRoute("/users/me");
        if (res.Success)
        {
            var json = JsonSerializer.Deserialize<UserBase>(res.Content, ApiService.Instance.options);
            if (json == null) return 0;
            await UserConfig.SaveAsync(json);
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}
