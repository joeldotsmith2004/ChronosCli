using Spectre.Console;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using System.Net;
using System.Text.Json;

public class ApiHandler
{
    public HttpClient Client;
    public AzureConfig Config;
    public JsonSerializerOptions options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        PropertyNameCaseInsensitive = true
    };

    private TokenCache TokenCache;
    private IPublicClientApplication App;


    private ApiHandler(HttpClient client, AzureConfig config, TokenCache tokenCache, IPublicClientApplication app)
    {
        Client = client;
        Config = config;
        TokenCache = tokenCache;
        App = app;
    }

    public static async Task<ApiHandler> CreateAsync()
    {
        var client = new HttpClient();
        var config = new AzureConfig();

        var tokenCache = await TokenCache.CreateAsync();

        var app = PublicClientApplicationBuilder
            .Create(config.ClientId)
            .WithTenantId(config.TenantId)
            .WithRedirectUri("http://localhost")
            .Build();

        tokenCache.CacheHelper.RegisterCache(app.UserTokenCache);
        return new ApiHandler(client, config, tokenCache, app);
    }

    public async Task AddAccessToken()
    {
        try
        {
            var accounts = await App.GetAccountsAsync();
            var result = await App.AcquireTokenSilent(Config.Scopes, accounts.FirstOrDefault()).ExecuteAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        }
        catch
        {
            var result = await App.AcquireTokenInteractive(Config.Scopes)
                .WithPrompt(Prompt.SelectAccount) 
                .ExecuteAsync();

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
        }
    }

    public async Task<ApiResult> GetRoute(string route)
    {
        var res = await Client.GetAsync($"{Config.ApiUrl}api{route}");
        var content = await res.Content.ReadAsStringAsync();

        return new ApiResult
        {
            Success = res.IsSuccessStatusCode,
            Content = content,
            StatusCode = res.StatusCode
        };
    }

    public async Task<ApiResult> PutRoute(string route, HttpContent content)
    {
        var res = await Client.PutAsync($"{Config.ApiUrl}api{route}", content);
        var resContent = await res.Content.ReadAsStringAsync();

        return new ApiResult
        {
            Success = res.IsSuccessStatusCode,
            Content = resContent,
            StatusCode = res.StatusCode
        };
    }

    public async Task<ApiResult> PostRoute(string route, HttpContent content)
    {
        var res = await Client.PostAsync($"{Config.ApiUrl}api{route}", content);
        var resContent = await res.Content.ReadAsStringAsync();

        return new ApiResult
        {
            Success = res.IsSuccessStatusCode,
            Content = resContent,
            StatusCode = res.StatusCode
        };
    }

    public async Task<ApiResult> PatchRoute(string route, HttpContent content)
    {
        var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"{Config.ApiUrl}api{route}")
        {
            Content = content
        };
        var res = await Client.SendAsync(req);
        var resContent = await res.Content.ReadAsStringAsync();

        return new ApiResult
        {
            Success = res.IsSuccessStatusCode,
            Content = resContent,
            StatusCode = res.StatusCode
        };
    }

    public async Task<ApiResult> DeleteRoute(string route)
    {
        var res = await Client.DeleteAsync($"{Config.ApiUrl}api{route}");
        var content = await res.Content.ReadAsStringAsync();

        return new ApiResult
        {
            Success = res.IsSuccessStatusCode,
            Content = content,
            StatusCode = res.StatusCode
        };
    }
}


public class ApiResult
{
    public bool Success { get; set; }
    public string Content { get; set; } = "";
    public HttpStatusCode StatusCode { get; set; }
}
