using Spectre.Console;
using Spectre.Console.Cli;
using System.Text.Json;
using Backend.Core.Schemas;

public class GetProjects : AsyncCommand<GetProjects.Settings>
{
    public class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var user = await UserConfig.LoadAsync();
        if (user == null) return 0;

        var res = await ApiService.Instance.GetRoute($"/task-user/{user.Id}");
        if (res.Success)
        {
            var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, ApiService.Instance.options);
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
        var user = await UserConfig.LoadAsync();
        if (user == null) return 0;

        var res = await ApiService.Instance.GetRoute($"/task-user/{user.Id}");
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
        var user = await UserConfig.LoadAsync();
        if (user == null) return 0;

        var res = await ApiService.Instance.GetRoute($"/task-user/{user.Id}");
        if (res.Success)
        {
            var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, JsonOptions);
            if (projects == null) return 0;

            var table = new Table();
            table.AddColumn("Project");
            table.AddColumn("Task Id");
            table.AddColumn("Name");
            table.AddColumn("Purchase Order");
            foreach (var project in projects)
            {
                foreach (var po in project.PurchaseOrders)
                {
                    foreach (var task in po.Tasks)
                    {
                        if (task.CanAddEntries) table.AddRow(project.Name, $"[green]{task.Id.ToString()}[/]", task.Name, po.Name);
                    }
                }
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
