using Spectre.Console;
using Spectre.Console.Cli;
using Backend.Core.Schemas;

public class GetEntries : AsyncCommand<GetEntries.Settings>
{
    public class Settings : CommandSettings
    {
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        // /time-entry/<userid>?minDate=<monday>&maxDate=<sunday> generally like this 
        // can just change the dates to whatever size you want
        var res = await ApiService.Instance.GetRoute("/time-entry/166?minDate=2025-09-15&maxDate=2025-09-21"); // TODO remove hard coded ID maybe store in config
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

public class AddEntry : AsyncCommand<AddEntry.Settings>
{
    public class Settings : CommandSettings
    {

        [CommandArgument(0, "<TaskId>")]
        public int TaskId { get; set; }

        [CommandArgument(1, "[Hours]")]
        public float Hours { get; set; } = 8; // user default hours to be set

        [CommandArgument(2, "[Message]")]
        public string? Message { get; set; } = null;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var payload = new TimeEntry
        {
            UserId = 166,
            Date = DateOnly.FromDateTime(DateTime.Now),
            TaskId = settings.TaskId,
            SubTaskId = null,
            Hours = settings.Hours,
            Comment = settings.Message
        };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var res = await ApiService.Instance.PostRoute("/time-entry", content);
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
