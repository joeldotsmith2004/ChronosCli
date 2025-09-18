using System.CommandLine;
using Spectre.Console;

public class UserCommands
{
    public void AddUserCommands()
    {
        GetMe();

    }

    public void GetMe()
    {
        var getUserCmd = new Command("me", "Fetch my user info");
        getUserCmd.SetHandler(async () =>
        {
            var res = await ApiService.Instance.GetRoute("/users/me");
            if (res.Success)
            {
                AnsiConsole.MarkupLine($"[green]Response:[/] {res.Content}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            }
        });
        RootCommandService.Instance.AddCommand(getUserCmd);
    }
}
