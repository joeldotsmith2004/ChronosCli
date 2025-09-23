using Spectre.Console.Cli;
using Terminal.Gui;

public sealed class TuiCommand : Command<TuiCommand.Settings>
{
    public sealed class Settings : CommandSettings { }

    public override int Execute(CommandContext context, Settings settings)
    {
        Application.Init();
        try
        {
            Application.Run(new Tui());
        }
        finally
        {
            Application.Shutdown();
        }
        return 0;
    }
}
