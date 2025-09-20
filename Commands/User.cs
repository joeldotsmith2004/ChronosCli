using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

public class UserCommand : AsyncCommand<UserCommand.Settings>
{
    public class Settings : CommandSettings
    {
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.GetRoute("/users/me");
        if (res.Success)
        {
            AnsiConsole.Write(new JsonText(res.Content));
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}

public class SetDefaultHours : AsyncCommand<SetDefaultHours.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<hours>")]
        public float Hours { get; set; } = 8;
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.PutRoute($"/users/default-entry-hours?defaultEntryHours={settings.Hours}", new StringContent(string.Empty));
        if (res.Success)
        {
            AnsiConsole.MarkupLine($"[green]Success: Default entry hours set to {settings.Hours}[/]");
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}
