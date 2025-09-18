using System.CommandLine;
using Spectre.Console;
using System.Net.Http.Headers;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
using System.Net;

public static class RootCommandService
{
    public static RootCommand Instance { get; private set; } = null!;

    public static void Init()
    {
        Instance = new RootCommand("CLI for Enco Chronos");
        AddCommands();
    }


    public static void AddCommands()
    {
        new UserCommands().AddUserCommands();
        new GeneralCommands().AddGeneralCommands();
        new ProjectsCommands().AddProjectsCommands();
        new HoursCommands().AddHoursCommands();

        return;
    }
}
