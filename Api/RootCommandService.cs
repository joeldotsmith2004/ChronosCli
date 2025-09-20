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
              .WithDescription("Adds a time entry")
              .WithExample(new[] { "add <task-id> [hours] [message]" });

            config.AddCommand<GetEntries>("entries")
              .WithDescription("Gets this weeks time entries");

            config.AddCommand<RemoveEntry>("remove")
              .WithDescription("Removes a Time Entry")
              .WithExample(new[] { "remove <entry-id>" })
              .WithAlias("rm");

            config.AddCommand<GetPos>("pos")
              .WithDescription("Gets Assigned Purchase Orders");

            config.AddCommand<GetProjects>("projects")
              .WithDescription("Gets Assigned Projects");

            config.AddCommand<GetTasks>("tasks")
              .WithDescription("Gets Assigned Tasks");

            config.AddCommand<SetDefaultHours>("set-default-hours")
              .WithDescription("Sets your default hours");
        });
    }
}
