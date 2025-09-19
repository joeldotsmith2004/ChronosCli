using Spectre.Console.Cli;

public static class RootCommandService
{
    public static CommandApp Instance { get; private set; } = new CommandApp();

    public static void Init()
    {
        Instance.Configure(config =>
        {
            config.AddCommand<UserCommand>("me")
              .WithDescription("Gets User Information");
            config.AddCommand<AppSettings>("app-settings")
              .WithDescription("Gets Chronos App Settings");
            config.AddCommand<AddEntry>("add")
              .WithDescription("Adds a time entry");
            config.AddCommand<GetEntries>("entries")
              .WithDescription("Gets this weeks time entries");
            config.AddCommand<GetPos>("pos")
              .WithDescription("Gets Assigned Purchase Orders");
            config.AddCommand<GetProjects>("projects")
              .WithDescription("Gets Assigned Projects");
            config.AddCommand<GetTasks>("tasks")
              .WithDescription("Gets Assigned Tasks");
        });
    }
}
