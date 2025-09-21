using Terminal.Gui;
using Backend.Core.Schemas;
using System.Text.Json;

public class Tui : Terminal.Gui.Window
{
    private const int gap = 1;

    private UserBase userData = null!;
    private List<TimeEntryGet> entryData = null!;
    private List<ProjectAssignedDto> tasksData = null!;

    private DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-(7 + (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday) % 7));
    public DateOnly endDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek % 7));

    private SpinnerView spinner = null!;
    private Entries entries = null!;
    private Tasks tasks = null!;
    private BottomBar statusBar = null!;

    public Tui()
    {
        SetTheme();
        // base settings
        this.Y = 0;
        this.X = 0;
        this.BorderStyle = LineStyle.None;
        this.ColorScheme = Colors.ColorSchemes["TopLevel"];

        entries = new Entries();
        tasks = new Tasks(entries);
        statusBar = new BottomBar();


        // loading spinner
        spinner = new SpinnerView()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            AutoSpin = true,
            ColorScheme = Colors.ColorSchemes["Base"]
        };
        this.Add(spinner);


        _ = LoadUserAsync();
    }

    public void FinishedLoading()
    {
        this.Remove(spinner);

        this.Add(entries);
        this.Add(tasks);
        this.Add(statusBar);

        entries.entriesTable.SetFocus();
    }

    public bool IsFinishedLoading()
    {
        return (entries != null || tasks != null || statusBar != null);
    }

    private async Task LoadUserAsync()
    {
        var res = await UserConfig.LoadAsync();
        if (res == null) return;
        userData = res;

        Application.Invoke(() =>
        {
            statusBar.userNameItem.Text = $"{userData.Name}";
            statusBar.emailItem.Text = $"{userData.Email}";
            statusBar.SetNeedsDraw();
        });

        // now that we have the user we can load the rest of the data
        _ = LoadEntriesAsync();
        _ = LoadTasksAsync();
    }

    private async Task LoadEntriesAsync()
    {
        var res = await ApiService.Instance.GetRoute($"/time-entry/{userData.Id}?minDate={startDate.ToString("yyyy-MM-dd")}&maxDate={endDate.ToString("yyyy-MM-dd")}");
        if (!res.Success) return;
        var data = JsonSerializer.Deserialize<List<TimeEntryGet>>(res.Content, ApiService.Instance.options);
        if (data == null) return;
        entryData = data;

        Application.Invoke(() =>
        {
            foreach (var entry in entryData)
            {
                entries.entriesData.Rows.Add(entry.Id, entry.TaskId, entry.Hours, entry.Comment);
                entries.entriesTable.Update();
                entries.NeedsDraw = true;
            }
        });

        if (IsFinishedLoading()) FinishedLoading();
    }

    private async Task LoadTasksAsync()
    {
        var res = await ApiService.Instance.GetRoute($"/task-user/{userData.Id}");
        if (!res.Success) return;
        var data = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, ApiService.Instance.options);
        if (data == null) return;
        tasksData = data;

        Application.Invoke(() =>
        {
            tasks.RemoveAll();
            foreach (var project in tasksData)
            {
                foreach (var po in project.PurchaseOrders)
                {
                    foreach (var task in po.Tasks)
                    {
                        if (task.CanAddEntries)
                        {
                            tasks.taskData.Rows.Add(project.Name, task.Id.ToString(), task.Name, po.Name);
                            tasks.taskTable.Update();
                            tasks.NeedsDraw = true;
                        }
                    }
                }
            }
            tasks.Add(tasks.taskTable);
        });

        if (IsFinishedLoading()) FinishedLoading();
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

