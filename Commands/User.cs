using System.CommandLine;
using Spectre.Console;
using System.Text.Json;
using Backend.Core.Schemas;

public class UserCommands
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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
                AnsiConsole.MarkupLine($"[green]Response:[/] {Markup.Escape(res.Content)}");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error {res.StatusCode}[/]");
            }
        });
        RootCommandService.Instance.AddCommand(getUserCmd);
    }




}
