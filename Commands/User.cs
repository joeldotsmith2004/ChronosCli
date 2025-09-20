using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;
using System.Text.Json;

public class UserCommand : AsyncCommand<UserCommand.Settings>
{
    public class Settings : CommandSettings
    {
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await UserConfig.LoadAsync();
        if (res == null) return 0;

        var json = JsonSerializer.Serialize(res, ApiService.Instance.options);
        AnsiConsole.Write(new JsonText(json));
        return 1;
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
