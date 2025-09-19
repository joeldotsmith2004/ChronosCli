using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;
using Backend.Core.Schemas;

public class GetProjects : AsyncCommand<GetProjects.Settings>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.GetRoute("/task-user/166"); // TODO remove hard coded ID maybe store in config
        if (res.Success)
        {
            var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, JsonOptions);
            if (projects == null) return 0;
            foreach (var project in projects)
            {
                AnsiConsole.MarkupLine($"[green]{Markup.Escape($"[{project.Id}]")}[/] {Markup.Escape(project.Name)}");
            }
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}


public class GetPos : AsyncCommand<GetPos.Settings>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.GetRoute("/task-user/166"); // TODO remove hard coded ID maybe store in config
        if (res.Success)
        {
            var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, JsonOptions);
            if (projects == null) return 0;
            foreach (var project in projects)
            {
                foreach (var po in project.PurchaseOrders)
                {
                    AnsiConsole.MarkupLine($"[yellow]{Markup.Escape($"[{po.Id}]")}[/] {Markup.Escape(po.Name)}");
                }
            }
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}

public class GetTasks : AsyncCommand<GetTasks.Settings>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var res = await ApiService.Instance.GetRoute("/task-user/166");
        if (res.Success)
        {
            var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, JsonOptions);
            if (projects == null) return 0;
            foreach (var project in projects)
            {
                foreach (var po in project.PurchaseOrders)
                {
                    AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(po.Name)}[/]");
                    foreach (var task in po.Tasks)
                    {
                        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape($"[{task.Id}]")}[/] {Markup.Escape(task.Name)}");
                    }
                }
            }
            return 1;
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            return 0;
        }
    }
}
