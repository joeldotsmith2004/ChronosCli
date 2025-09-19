using System.CommandLine;
using Spectre.Console;
using System.Text.Json;
using Backend.Core.Schemas;

public class HoursCommands
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public void AddHoursCommands()
    {
        CurTimeEntries();

    }

    public void CurTimeEntries()
    {
        var cmd = new Command("entries", "Returns Current Time Entries");
        cmd.SetHandler(async () =>
        {
            // /time-entry/<userid>?minDate=<monday>&maxDate=<sunday> generally like this 
            // can just change the dates to whatever size you want
            var res = await ApiService.Instance.GetRoute("/time-entry/166?minDate=2025-09-15&maxDate=2025-09-21"); // TODO remove hard coded ID maybe store in config
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
