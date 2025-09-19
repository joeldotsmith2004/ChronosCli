using Spectre.Console;
using Spectre.Console.Cli;

public class AppSettings : AsyncCommand<AppSettings.Settings>
{
    public class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.GetRoute("/app-settings/basic");
        if (res.Success)
        {
            AnsiConsole.MarkupLine($"[green]Response:[/] {Markup.Escape(res.Content)}");
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}
