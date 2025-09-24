using System.Text.Json;
using Terminal.Gui;
using Spectre.Console;

public static class ThemeStore
{
    static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".chronos");

    static readonly string ConfigPath = Path.Combine(ConfigDir, "theme.json");

    public static void Save(Theme info)
    {
        Directory.CreateDirectory(ConfigDir);
        var json = JsonSerializer.Serialize(info, ApiService.Instance.options);
        File.WriteAllText(ConfigPath, json);
        Console.WriteLine($"Saved theme config to path: {ConfigPath}");
    }

    public static void Load()
    {
        if (!File.Exists(ConfigPath))
        {
            AnsiConsole.MarkupLine("No theme config found. Setting a default...");
            Save(new Theme());
        }
        var json = File.ReadAllText(ConfigPath);
        var theme = JsonSerializer.Deserialize<Theme>(json, ApiService.Instance.options);
        if (theme == null) return;
        SetTheme(theme);
    }

    public static void SetTheme(Theme theme)
    {

        Terminal.Gui.Color WindowHighlightColour = new Terminal.Gui.Color(theme.WindowHighlight.R, theme.WindowHighlight.G, theme.WindowHighlight.B);
        Terminal.Gui.Color WindowBaseColour = new Terminal.Gui.Color(theme.WindowBase.R, theme.WindowBase.G, theme.WindowBase.B);
        Terminal.Gui.Color DialogColour = new Terminal.Gui.Color(theme.Dialog.R, theme.Dialog.G, theme.Dialog.B);
        Terminal.Gui.Color BackgroundColour = new Terminal.Gui.Color(theme.Background.R, theme.Background.G, theme.Background.B);
        Terminal.Gui.Color MenuBarText = new Terminal.Gui.Color(theme.MenuBarText.R, theme.MenuBarText.G, theme.MenuBarText.B);
        Terminal.Gui.Color MenuBarBackground = new Terminal.Gui.Color(theme.MenuBarBackground.R, theme.MenuBarBackground.G, theme.MenuBarBackground.B);

        Colors.ColorSchemes["Base"] = new Terminal.Gui.ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(WindowBaseColour, BackgroundColour),
            Focus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Terminal.Gui.Color.BrightCyan, BackgroundColour),
            HotFocus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.BrightCyan)
        };

        Colors.ColorSchemes["MainWindowHighlight"] = new Terminal.Gui.ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(WindowHighlightColour, BackgroundColour),
            Focus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Terminal.Gui.Color.BrightCyan, BackgroundColour),
            HotFocus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.BrightCyan)
        };

        Colors.ColorSchemes["TopLevel"] = new Terminal.Gui.ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(BackgroundColour, BackgroundColour)
        };

        Colors.ColorSchemes["InputField"] = new Terminal.Gui.ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Terminal.Gui.Color.White, Terminal.Gui.Color.DarkGray),
            Focus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Terminal.Gui.Color.Cyan, BackgroundColour),
            HotFocus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Blue)
        };

        Colors.ColorSchemes["Dialog"] = new Terminal.Gui.ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(DialogColour, BackgroundColour),
            Focus = new Terminal.Gui.Attribute(Terminal.Gui.Color.White, BackgroundColour),
            HotNormal = new Terminal.Gui.Attribute(Terminal.Gui.Color.BrightYellow, Terminal.Gui.Color.DarkGray),
            HotFocus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.BrightYellow)
        };

        Colors.ColorSchemes["Menu"] = new Terminal.Gui.ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(MenuBarText, MenuBarBackground),
            Focus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Terminal.Gui.Color.BrightCyan, BackgroundColour),
            HotFocus = new Terminal.Gui.Attribute(Terminal.Gui.Color.Black, Terminal.Gui.Color.BrightCyan)
        };
    }

}
