using System.CommandLine;
using Spectre.Console;

public class UserCommands
{
    public void AddUserCommands()
    {
        GetMe();
        CurTimeEntries();
        GetProjects();
    }

    public void GetMe()
    {
        var getUserCmd = new Command("me", "Fetch my user info");
        getUserCmd.SetHandler(async () =>
        {
            var res = await ApiService.Instance.GetRoute("/users/me");
            if (res.Success)
            {
                AnsiConsole.MarkupLine($"[green]Response:[/] {Markup.Escape(res.Content)}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            }
        });
        RootCommandService.Instance.AddCommand(getUserCmd);
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

    public void GetProjects()
    {
        var cmd = new Command("projects", "Returns Assigned Projects");
        cmd.SetHandler(async () =>
        {
            var res = await ApiService.Instance.GetRoute("/task-user/166"); // TODO remove hard coded ID maybe store in config
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
