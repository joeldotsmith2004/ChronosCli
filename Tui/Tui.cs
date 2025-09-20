using Terminal.Gui;

public class Tui : Terminal.Gui.Window
{
    private const int gap = 1;

    private Window entries = null!;
    private Window tasks = null!;
    private StatusBar statusBar = null!;
    private Label userNameItem = null!;
    private Label emailItem = null!;
    private Label actionItem = null!;

    public Tui()
    {

        SetTheme();

        // base settings
        this.Y = 0;
        this.X = 0;
        this.BorderStyle = LineStyle.None;
        this.ColorScheme = Colors.ColorSchemes["TopLevel"];

        AddWindows();
        CreateStatusBar();

        this.Add(entries);
        this.Add(tasks);
        this.Add(statusBar);
        _ = LoadUserAsync();
    }

    public void AddWindows()
    {
        entries = new Window()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(gap),
            Height = Dim.Percent(60),
            BorderStyle = LineStyle.Rounded,
            Title = "1",
            HotKey = Key.D1
        };

        tasks = new Window()
        {
            X = gap,
            Y = Pos.Bottom(entries),
            Width = Dim.Fill(gap),
            BorderStyle = LineStyle.Rounded,
            Height = Dim.Fill(gap),
            Title = "2",
            HotKey = Key.D2
        };
    }

    public View CreateStatusBar()
    {
        statusBar = new StatusBar()
        {
            X = 0,
            Y = Pos.Bottom(tasks),
            Width = Dim.Fill(),
            AlignmentModes = AlignmentModes.AddSpaceBetweenItems
        };

        userNameItem = new Label() { Text = "UserName", ColorScheme = Colors.ColorSchemes["Green"] };
        emailItem = new Label() { Text = "Email", ColorScheme = Colors.ColorSchemes["Yellow"] };
        actionItem = new Label() { Text = "" };
        statusBar.Add(userNameItem);
        statusBar.Add(emailItem);
        statusBar.Add(actionItem);
        return statusBar;
    }

    private async Task LoadUserAsync()
    {
        var user = await UserConfig.LoadAsync();
        if (user == null) return;

        Application.Invoke(() =>
        {
            userNameItem.Text = $"{user.Name}";
            emailItem.Text = $"{user.Email}";
            statusBar.SetNeedsDraw();
        });
    }

    public void SetTheme()
    {
        Colors.ColorSchemes["Base"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Black, Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightCyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Black, Color.BrightCyan)
        };

        Colors.ColorSchemes["TopLevel"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Black, Color.Black)
        };

        Colors.ColorSchemes["Green"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Green, Color.Black)
        };

        Colors.ColorSchemes["Yellow"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Yellow, Color.Black)
        };

        Colors.ColorSchemes["Blue"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Blue, Color.Black)
        };

        Colors.ColorSchemes["Dialog"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.DarkGray),
            Focus = new Terminal.Gui.Attribute(Color.Black, Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray),
            HotFocus = new Terminal.Gui.Attribute(Color.Black, Color.BrightYellow)
        };

        Colors.ColorSchemes["Menu"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Black, Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightCyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Black, Color.BrightCyan)
        };

        Colors.ColorSchemes["Error"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.Black, Color.BrightRed)
        };
    }
}

