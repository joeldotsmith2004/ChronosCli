using Terminal.Gui;
using System.Data;
using Backend.Core.Schemas;
using System.Text.Json;
using System.Text;

public class Entries : Window
{
    private const int gap = 1;

    public TableView entriesTable = null!;
    public DataTableSource entriesSource = null!;
    public DataTable entriesData = null!;

    private SpinnerView loader = new SpinnerView

    {
        X = Pos.Center(),
        Y = Pos.Center(),
        AutoSpin = true,
        ColorScheme = Colors.ColorSchemes["Base"]
    };

    public Entries()
    {
        this.X = 1;
        this.Y = 1;
        this.Width = Dim.Fill(gap);
        this.Height = Dim.Percent(60);
        this.BorderStyle = LineStyle.Rounded;
        this.Title = "1";

        entriesData = new DataTable();
        entriesData.Columns.Add("Project", typeof(string));
        entriesData.Columns.Add("Task", typeof(string));
        entriesData.Columns.Add("Date", typeof(DateOnly));
        entriesData.Columns.Add("Hours", typeof(float));
        entriesData.Columns.Add("Comment", typeof(string));

        entriesSource = new DataTableSource(entriesData);
        entriesTable = new TableView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            FullRowSelect = true,
            MultiSelect = false,
            Table = entriesSource,
            HotKey = '1',
            CollectionNavigator = null,
        };

        entriesTable.KeyDown += (view, keyEvent) =>
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

        entriesTable.KeyBindings.Add(Key.H, Command.Left);
        entriesTable.KeyBindings.Add(Key.J, Command.Down);
        entriesTable.KeyBindings.Add(Key.K, Command.Up);
        entriesTable.KeyBindings.Add(Key.L, Command.Right);

        this.Add(entriesTable);
    }

    private void RemoveLoading()
    {
        this.Remove(loader);
        this.Add(entriesTable);
        this.NeedsDraw = true;
    }

    private void SetLoading()
    {
        this.RemoveAll();
        this.Add(loader);
        this.NeedsDraw = true;
    }

    private void DeleteEntry()
    {
        if (Store.Instance.Entries.Count <= entriesTable.SelectedRow || entriesTable.SelectedRow < 0) return;
        var n = MessageBox.YesNo("Delete Entry", "Are you sure you want to delete this entry it cannot be undone?");
        if (n == 0)
        {
            SetLoading();
            var row = entriesTable.SelectedRow;
            Task.Run(async () =>
            {
                var res = await ApiService.Instance.DeleteRoute($"/time-entry/{Store.Instance.Entries[row].Id}");
                if (res.Success)
                {
                    Application.Invoke(() =>
                    {
                        Store.Instance.Entries.RemoveAt(row);
                        entriesData.Rows.RemoveAt(row);
                        entriesTable.SelectedRow = Math.Max(0, row - 1);
                        entriesTable.NeedsDraw = true;
                        entriesTable.Update();
                    });
                }
            });
            RemoveLoading();
        }
    }

    private void EditEntry()
    {
        if (Store.Instance.Entries.Count() <= entriesTable.SelectedRow || entriesTable.SelectedRow < 0) return;
        var updated = EditDialog.Edit(entriesTable.SelectedRow);
        if (updated != null)
        {
            SetLoading();
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
                        Store.Instance.Entries[entriesTable.SelectedRow] = obj;
                        entriesData.Rows.Clear();
                        foreach (var entry in Store.Instance.Entries)
                        {
                            projectData = Store.Instance.TaskToProject[entry.TaskId];
                            entriesData.Rows.Add(projectData.Item1, projectData.Item3, entry.Date, entry.Hours, entry.Comment);
                            entriesTable.Update();
                            NeedsDraw = true;
                        }
                    }
                }
            });
            RemoveLoading();
        }
    }

}
