public static class ApiService
{
    public static ApiHandler Instance { get; private set; } = null!;

    public static async Task InitAsync()
    {
        Instance = await ApiHandler.CreateAsync();
        await Instance.AddAccessToken();
    }
}
