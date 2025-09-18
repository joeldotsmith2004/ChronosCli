using System.CommandLine;
using Spectre.Console;

public class GeneralCommands
{
    public void AddGeneralCommands()
    {
        Settings();
    }

    public void Settings()
    {
        var cmd = new Command("app-settings", "Gets Chronos App Settings");
        cmd.SetHandler(async () =>
        {
            var res = await ApiService.Instance.GetRoute("/app-settings/basic");
            if (res.Success)
            {
                AnsiConsole.MarkupLine($"[green]Response:[/] {Markup.Escape(res.Content)}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            }
        });
        RootCommandService.Instance.AddCommand(cmd);
    }
}
