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
            Application.KeyBindings.Remove(Key.Tab);
            Application.KeyBindings.Remove(Key.Tab.WithShift);
            Application.KeyBindings.Add(Key.Tab, Terminal.Gui.Command.NextTabGroup);
            Application.KeyBindings.Add(Key.Tab.WithShift, Terminal.Gui.Command.PreviousTabGroup);

            Application.Run(new Tui());
        }
        finally
        {
            Application.Shutdown();
        }
        return 0;
    }
}
