using Terminal.Gui;
using Backend.Core.Schemas;
using System.Text.Json;

public class Tui : Window
{
    private const int gap = 1;

    public SpinnerView spinner = null!;
    public Entries entries = null!;
    public Tasks tasks = null!;
    public BottomBar statusBar = null!;

    public Tui()
    {
        SetTheme();
        // base settings
        this.Y = 0;
        this.X = 0;
        this.BorderStyle = LineStyle.None;
        this.ColorScheme = Colors.ColorSchemes["TopLevel"];

        entries = new Entries();
        entries.HasFocusChanged += (old, newFocused) =>
        {
            if (newFocused.NewValue == true)
            {
                statusBar.SelectEntries();
            }
        };


        tasks = new Tasks(entries);
        tasks.HasFocusChanged += (old, newFocused) =>
        {
            if (newFocused.NewValue == true)
            {
                statusBar.SelectTasks();
            }
        };

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
        Store.Instance.User = res;

        Application.Invoke(() =>
        {
            statusBar.infoLabel.Text = $"{Store.Instance.User.Name} - {Store.Instance.User.Email}";
            statusBar.SetNeedsDraw();
        });

        // now that we have the user we can load the rest of the data
        _ = LoadTasksAsync();
    }

    private async Task LoadEntriesAsync()
    {
        var res = await ApiService.Instance.GetRoute($"/time-entry/{Store.Instance.User.Id}?minDate={Store.Instance.startDate.ToString("yyyy-MM-dd")}&maxDate={Store.Instance.endDate.ToString("yyyy-MM-dd")}");
        if (!res.Success) return;
        var data = JsonSerializer.Deserialize<List<TimeEntryGet>>(res.Content, ApiService.Instance.options);
        if (data == null) return;
        Store.Instance.Entries = data;
        Tuple<string, string, string> projectData;

        Application.Invoke(() =>
        {
            foreach (var entry in Store.Instance.Entries)
            {
                projectData = Store.Instance.TaskToProject[entry.TaskId];
                entries.entriesData.Rows.Add(projectData.Item1, projectData.Item3, entry.Date, entry.Hours, entry.Comment);
                entries.entriesTable.Update();
                entries.NeedsDraw = true;
            }
        });

        if (IsFinishedLoading()) FinishedLoading();
    }

    private async Task LoadTasksAsync()
    {
        var res = await ApiService.Instance.GetRoute($"/task-user/{Store.Instance.User.Id}");
        if (!res.Success) return;
        var data = JsonSerializer.Deserialize<List<ProjectAssignedDto>>(res.Content, ApiService.Instance.options);
        if (data == null) return;
        Store.Instance.Tasks = data;

        Application.Invoke(() =>
        {
            tasks.RemoveAll();
            foreach (var project in Store.Instance.Tasks)
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
                        Store.Instance.TaskToProject.Add(task.Id, new Tuple<string, string, string>(project.Name, po.Name, task.Name));
                    }
                }
            }
            tasks.Add(tasks.taskTable);
        });

        _ = LoadEntriesAsync();
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

        Colors.ColorSchemes["InputField"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.DarkGray),
            Focus = new Terminal.Gui.Attribute(Color.Black, Color.Gray),
            HotNormal = new Terminal.Gui.Attribute(Color.Cyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.Black, Color.Blue)
        };

        Colors.ColorSchemes["Dialog"] = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Black),
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

