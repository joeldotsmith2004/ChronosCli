using Terminal.Gui;

public enum Focused_View
{
    Entries,
    Tasks
}

public class BottomBar : StatusBar
{
    public Label infoLabel = null!;
    public Label hourCount = null!;

    public Focused_View CurrentFocused = Focused_View.Entries;

    public List<Label> entryItems = new List<Label>();
    public List<Label> taskItems = new List<Label>();

    public BottomBar()
    {
        this.X = 0;
        this.Width = Dim.Fill();
        this.AlignmentModes = AlignmentModes.AddSpaceBetweenItems;

        // default items
        infoLabel = new Label() { Text = "No Information" };
        hourCount = new Label() { Text = "0" };

        SetHourCount();


        // task specfic
        taskItems.Add(new Label() { Text = "Add (a)" });

        // entry specfic
        entryItems.Add(new Label() { Text = "Edit (e)" });
        entryItems.Add(new Label() { Text = "Delete (d)" });
        entryItems.Add(new Label() { Text = "Filter (f)" });

        this.Add(infoLabel);
    }

    public void RefreshBar()
    {
        switch (CurrentFocused)
        {
            case Focused_View.Entries:
                SelectEntries();
                break;
            case Focused_View.Tasks:
                SelectTasks();
                break;
        }
    }

    public void SetHourCount()
    {
        var total = 0.0;
        Store.Instance.Entries.ForEach(x => total += x.Hours);
        hourCount = new Label() { Text = total.ToString() };

        SetNeedsDraw();
    }

    public void SelectEntries()
    {
        RemoveAll();

        var leftGroup = new View { Width = Dim.Auto(), Height = 1 };
        var rightGroup = new View { Width = Dim.Auto(), Height = 1 };

        int x = 2;
        foreach (var entry in entryItems)
        {
            entry.X = x;
            leftGroup.Add(entry);
            x += entry.Text.GetColumns() + 2;
        }

        SetHourCount();
        int rx = 0;
        foreach (var r in new[] { hourCount, infoLabel })
        {
            r.X = rx;
            rightGroup.Add(r);
            rx += r.Text.GetColumns() + 2;
        }

        var spacer = new View
        {
            Width = Dim.Percent(100)! - Dim.Width(leftGroup) - Dim.Width(rightGroup),
            Height = 1
        };

        Add(leftGroup, spacer, rightGroup);
        SetNeedsDraw();
        CurrentFocused = Focused_View.Entries;
    }


    public void SelectTasks()
    {
        RemoveAll();

        var leftGroup = new View { Width = Dim.Auto(), Height = 1 };
        var rightGroup = new View { Width = Dim.Auto(), Height = 1 };

        int x = 2;
        foreach (var task in taskItems)
        {
            task.X = x;
            leftGroup.Add(task);
            x += task.Text.GetColumns() + 2;
        }

        int rx = 0;

        SetHourCount();

        foreach (var r in new[] { hourCount, infoLabel })
        {
            r.X = rx;
            rightGroup.Add(r);
            rx += r.Text.GetColumns() + 2;
        }

        var spacer = new View
        {
            Width = Dim.Percent(100)! - Dim.Width(leftGroup) - Dim.Width(rightGroup),
            Height = 1
        };

        Add(leftGroup, spacer, rightGroup);
        SetNeedsDraw();
        CurrentFocused = Focused_View.Tasks;
    }
}
