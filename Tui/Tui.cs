using Terminal.Gui;
using Backend.Core.Schemas;
using System.Text.Json;
using System.Text;

public class Tui : Window
{
    private const int gap = 1;
    private Entries entries = null!;
    private Tasks tasks = null!;
    private BottomBar statusBar = null!;

    private SpinnerView loader = new SpinnerView
    {
        X = Pos.Center(),
        Y = Pos.Center(),
        AutoSpin = true,
    };

    public Tui()
    {
        SetTheme();
        loader.ColorScheme = Colors.ColorSchemes["Base"];

        // base settings
        this.Y = 0;
        this.X = 0;
        this.BorderStyle = LineStyle.None;
        this.ColorScheme = Colors.ColorSchemes["TopLevel"];

        entries = new Entries();
        entries.entriesTable.KeyDown += (view, keyEvent) =>
        {
            if (keyEvent == Key.E)
            {
                EditEntry();
                keyEvent.Handled = true;
            }

            else if (keyEvent == Key.D)
            {
                DeleteEntry();
                keyEvent.Handled = true;
            }
        };
        entries.HasFocusChanged += (old, newFocused) =>
        {
            if (newFocused.NewValue == true)
            {
                statusBar.SelectEntries();
            }
        };


        // tasks
        tasks = new Tasks(entries);
        tasks.taskTable.KeyDown += (view, keyEvent) =>
        {
            if (keyEvent == Key.A)
            {
                CreateEntry();
                keyEvent.Handled = true;
            }
        };
        tasks.HasFocusChanged += (old, newFocused) =>
        {
            if (newFocused.NewValue == true)
            {
                statusBar.SelectTasks();
            }
        };

        statusBar = new BottomBar();


        // loading spinner
        this.Add(loader);

        _ = LoadUserAsync();
    }

    #region Loading
    private void RemoveLoading(View view, View toAdd)
    {
        view.Remove(loader);
        view.Add(toAdd);
        this.NeedsDraw = true;
    }

    private void SetLoading(View view)
    {
        view.RemoveAll();
        view.Add(loader);
        this.NeedsDraw = true;
    }

    public void FinishedLoading()
    {
        this.Remove(loader);

        this.Add(entries);
        this.Add(tasks);
        this.Add(statusBar);

        entries.entriesTable.SetFocus();
    }

    public bool IsFinishedLoading()
    {
        return (entries != null || tasks != null || statusBar != null);
    }

    #endregion

    #region Data

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
        Store.Instance.Entries = data.OrderByDescending(x => x.Date).ToList();
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

        Application.Invoke(() =>
        {
            tasks.RemoveAll();
            foreach (var project in data)
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
                            Store.Instance.Tasks.Add(task);
                        }
                        Store.Instance.TaskToProject.Add(task.Id, new Tuple<string, string, string>(project.Name, po.Name, task.Name));
                    }
                }
            }
            tasks.Add(tasks.taskTable);
        });

        _ = LoadEntriesAsync();
    }

    private void CreateEntry()
    {
        if (Store.Instance.Tasks.Count() <= tasks.taskTable.SelectedRow || tasks.taskTable.SelectedRow < 0) return;
        var updated = AddDialog.Add(tasks.taskTable.SelectedRow);
        if (updated != null)
        {
            SetLoading(tasks);
            Task.Run(async () =>
            {
                var json = JsonSerializer.Serialize(updated, ApiService.Instance.options);
                var payload = new StringContent(json, new UTF8Encoding(false), "application/json");
                var res = await ApiService.Instance.PostRoute("/time-entry", payload);
                if (res.Success)
                {
                    var obj = JsonSerializer.Deserialize<TimeEntryGet>(res.Content, ApiService.Instance.options);
                    if (obj != null)
                    {
                        var projectData = Store.Instance.TaskToProject[obj.TaskId];
                        Store.Instance.Entries.Insert(0, obj);
                        var row = entries.entriesData.NewRow();
                        row["Project"] = projectData.Item1;
                        row["Task"] = projectData.Item3;
                        row["Date"] = obj.Date;
                        row["Hours"] = obj.Hours;
                        row["Comment"] = obj.Comment;
                        entries.entriesData.Rows.InsertAt(row, 0);
                        entries.entriesTable.Update();
                        NeedsDraw = true;
                    }
                }
            });
            RemoveLoading(tasks, tasks.taskTable);
        }

    }

    private void DeleteEntry()
    {
        if (Store.Instance.Entries.Count <= entries.entriesTable.SelectedRow || entries.entriesTable.SelectedRow < 0) return;
        var n = MessageBox.YesNo("Delete Entry", "Are you sure you want to delete this entry it cannot be undone?");
        if (n == 0)
        {
            SetLoading(entries);
            var row = entries.entriesTable.SelectedRow;
            Task.Run(async () =>
            {
                var res = await ApiService.Instance.DeleteRoute($"/time-entry/{Store.Instance.Entries[row].Id}");
                if (res.Success)
                {
                    Application.Invoke(() =>
                    {
                        Store.Instance.Entries.RemoveAt(row);
                        entries.entriesData.Rows.RemoveAt(row);
                        entries.entriesTable.SelectedRow = Math.Max(0, row - 1);
                        entries.entriesTable.NeedsDraw = true;
                        entries.entriesTable.Update();
                    });
                }
            });
            RemoveLoading(entries, entries.entriesTable);
        }
    }

    private void EditEntry()
    {
        if (Store.Instance.Entries.Count() <= entries.entriesTable.SelectedRow || entries.entriesTable.SelectedRow < 0) return;
        var updated = EditDialog.Edit(entries.entriesTable.SelectedRow);
        if (updated != null)
        {
            SetLoading(entries);
            Task.Run(async () =>
            {
                var json = JsonSerializer.Serialize(updated, ApiService.Instance.options);
                var payload = new StringContent(json, new UTF8Encoding(false), "application/json");
                var res = await ApiService.Instance.PutRoute("/time-entry", payload);
                if (res.Success)
                {
                    var obj = JsonSerializer.Deserialize<TimeEntryGet>(res.Content, ApiService.Instance.options);
                    if (obj != null)
                    {
                        Tuple<string, string, string> projectData;
                        Store.Instance.Entries[entries.entriesTable.SelectedRow] = obj;
                        entries.entriesData.Rows.Clear();
                        foreach (var entry in Store.Instance.Entries)
                        {
                            projectData = Store.Instance.TaskToProject[entry.TaskId];
                            entries.entriesData.Rows.Add(projectData.Item1, projectData.Item3, entry.Date, entry.Hours, entry.Comment);
                            entries.entriesTable.Update();
                            NeedsDraw = true;
                        }
                    }
                }
            });
            RemoveLoading(entries, entries.entriesTable);
        }
    }


    #endregion

    #region Theme
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
    #endregion
}

