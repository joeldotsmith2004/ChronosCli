public class AzureConfig
{
    public string TenantId { get; set; } = "b8a07c69-3388-4583-8d63-3f6ca45416e2";
    public string ClientId { get; set; } = "8a1bfdb6-94ad-4dd0-a566-0eb5e11ce52c";
    public string[] Scopes { get; set; } = ["api://e640a1f5-5f1e-4fb3-9346-6209ed1e0d04/access_as_user"];
    public string ApiUrl { get; set; } = "https://time.enco.au/";
}
