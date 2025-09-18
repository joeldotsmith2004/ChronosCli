using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

public class TokenCache
{
    public MsalCacheHelper CacheHelper { get; private set; }

    private TokenCache(MsalCacheHelper cacheHelper)
    {
        CacheHelper = cacheHelper;
    }

    public static async Task<TokenCache> CreateAsync()
    {
        string cacheFileName = "msal_cache.dat";
        string cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "chronoscli");

        var storageProps = new StorageCreationPropertiesBuilder(cacheFileName, cacheDir)
            .WithLinuxKeyring(
                schemaName: "com.example.chronoscli",
                collection: "default",
                secretLabel: "MSAL token cache",
                attribute1: new KeyValuePair<string, string>("Version", "1"),
                attribute2: new KeyValuePair<string, string>("Product", "ChronosCLI"))
            .WithMacKeyChain(
                serviceName: "com.example.chronoscli",
                accountName: "MSALCache")
            .Build();

        var cacheHelper = await MsalCacheHelper.CreateAsync(storageProps);
        return new TokenCache(cacheHelper);
    }
}
