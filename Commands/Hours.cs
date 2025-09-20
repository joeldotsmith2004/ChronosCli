using Spectre.Console;
using Spectre.Console.Cli;
using Backend.Core.Schemas;
using System.Text.Json;
using System.Text;


public class GetEntries : AsyncCommand<GetEntries.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[Date]")] // defaults to this weeks monday
        public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(-(7 + (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday) % 7));

        [CommandArgument(1, "[Date]")] // defaults to this weeks sunday
        public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek % 7));
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var user = await UserConfig.LoadAsync();
        if (user == null) return 0;

        var res = await ApiService.Instance.GetRoute($"/time-entry/{user.Id}?minDate={settings.StartDate.ToString("yyyy-MM-dd")}&maxDate={settings.EndDate.ToString("yyyy-MM-dd")}");
        if (res.Success)
        {
            var entries = JsonSerializer.Deserialize<List<TimeEntryGet>>(res.Content, ApiService.Instance.options);
            if (entries == null) return 0;
            var table = new Table();
            table.ShowRowSeparators();
            table.AddColumn("Entry Id");
            table.AddColumn("Task Id");
            table.AddColumn("Hours");
            table.AddColumn("Comment");
            foreach (var entry in entries)
            {
                table.AddRow($"[red]{entry.Id}[/]", $"[green]{entry.TaskId.ToString()}[/]", entry.Hours.ToString(), entry.Comment ?? "No Comment");
            }
            AnsiConsole.Write(table);
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

        [CommandArgument(0, "<task-id>")]
        public int TaskId { get; set; }

        [CommandArgument(1, "[Hours]")]
        public float? Hours { get; set; } // defaults to user default hours

        [CommandArgument(2, "[Message]")]
        public string? Message { get; set; } = null;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {

        var user = await UserConfig.LoadAsync();
        if (user == null) return 0;

        var payload = new TimeEntryCreate
        {
            UserId = user.Id,
            Date = DateOnly.FromDateTime(DateTime.Now),
            TaskId = settings.TaskId,
            SubTaskId = (int?)null,
            Hours = settings.Hours ?? user.DefaultEntryHours,
            Comment = settings.Message
        };

        var json = System.Text.Json.JsonSerializer.Serialize(payload, ApiService.Instance.options);
        var content = new StringContent(json, new UTF8Encoding(false), "application/json");
        var res = await ApiService.Instance.PostRoute("/time-entry", content);

        if (res.Success)
        {
            AnsiConsole.MarkupLine($"[green]Success:[/] {Markup.Escape(res.Content)}");
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            AnsiConsole.MarkupLine($"[red]Error {res.Content}[/]");
            return 0;
        }
    }
}

public class RemoveEntry : AsyncCommand<RemoveEntry.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<entry-id>")]
        public int EntryId { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.DeleteRoute($"/time-entry/{settings.EntryId}");
        if (res.Success)
        {
            AnsiConsole.MarkupLine($"[green]Success: Entry {settings.EntryId} removed[/]");
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}
