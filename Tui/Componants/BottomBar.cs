using Terminal.Gui;

public class BottomBar : StatusBar
{
    public Label infoLabel = null!;

    public List<Label> entryItems = new List<Label>();
    public List<Label> taskItems = new List<Label>();

    public BottomBar()
    {
        this.X = 0;
        this.Width = Dim.Fill();
        this.AlignmentModes = AlignmentModes.AddSpaceBetweenItems;

        // default items
        infoLabel = new Label() { Text = "No Information" };

        // task specfic
        taskItems.Add(new Label() { Text = "Add (a)" });

        // entry specfic
        entryItems.Add(new Label() { Text = "Edit (e)" });
        entryItems.Add(new Label() { Text = "Delete (d)" });

        this.Add(infoLabel);
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

        int rx = 0;
        foreach (var r in new[] { infoLabel })
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
        foreach (var r in new[] { infoLabel })
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
    }
}
