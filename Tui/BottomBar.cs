using Terminal.Gui;

public class BottomBar : StatusBar
{
    public Label userNameItem = null!;
    public Label emailItem = null!;
    public Label actionItem = null!;

    public List<Label> entryItems = new List<Label>();
    public List<Label> taskItems = new List<Label>();

    public BottomBar()
    {
        this.X = 0;
        this.Width = Dim.Fill();
        this.AlignmentModes = AlignmentModes.AddSpaceBetweenItems;

        // default items
        userNameItem = new Label() { Text = "UserName" };
        emailItem = new Label() { Text = "Email" };
        actionItem = new Label() { Text = "" };

        // task specfic
        taskItems.Add(new Label() { Text = "Add (a)" });

        // entry specfic
        entryItems.Add(new Label() { Text = "Edit (e)" });
        entryItems.Add(new Label() { Text = "Delete (d)" });

        this.Add(userNameItem);
        this.Add(emailItem);
        this.Add(actionItem);
    }

    public void SelectEntries()
    {
        RemoveAll();

        var leftGroup = new View { Width = Dim.Auto(), Height = 1 };
        var rightGroup = new View { Width = Dim.Auto(), Height = 1 };

        int x = 0;
        foreach (var entry in entryItems)
        {
            entry.X = x;
            leftGroup.Add(entry);
            x += entry.Text.GetColumns() + 2;
        }

        int rx = 0;
        foreach (var r in new[] { userNameItem, emailItem, actionItem })
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

        int x = 0;
        foreach (var task in taskItems)
        {
            task.X = x;
            leftGroup.Add(task);
            x += task.Text.GetColumns() + 2;
        }

        int rx = 0;
        foreach (var r in new[] { userNameItem, emailItem, actionItem })
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
