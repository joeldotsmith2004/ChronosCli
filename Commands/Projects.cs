using System.CommandLine;
using Spectre.Console;
using System.Text.Json;
using Backend.Core.Schemas;


public class ProjectsCommands
{

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public void AddProjectsCommands()
    {
        GetProjects();
        GetPos();
    }

    public void GetProjects()
    {
        var cmd = new Command("projects", "Returns Assigned Projects");
        cmd.SetHandler(async () =>
        {
            var res = await ApiService.Instance.GetRoute("/task-user/166"); // TODO remove hard coded ID maybe store in config
            if (res.Success)
            {
                var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, JsonOptions);
                if (projects == null) return;
                foreach (var project in projects)
                {
                    AnsiConsole.MarkupLine($"[green]{Markup.Escape($"[{project.Id}]")}[/] {Markup.Escape(project.Name)}");
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            }
        });
        RootCommandService.Instance.AddCommand(cmd);
    }

    public void GetPos()
    {
        var cmd = new Command("pos", "Returns Assigned Purchase Orders");
        cmd.SetHandler(async () =>
        {
            var res = await ApiService.Instance.GetRoute("/task-user/166"); // TODO remove hard coded ID maybe store in config
            if (res.Success)
            {
                var projects = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, JsonOptions);
                if (projects == null) return;
                foreach (var project in projects)
                {
                    foreach (var po in project.PurchaseOrders)
                    {
                        AnsiConsole.MarkupLine($"[yellow]{Markup.Escape($"[{po.Id}]")}[/] {Markup.Escape(po.Name)}");
                    }
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            }
        });
        RootCommandService.Instance.AddCommand(cmd);
    }
}
