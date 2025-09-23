using Terminal.Gui;
using Terminal.Gui.TextValidateProviders;
using Backend.Core.Schemas;
using System.Globalization;

public static class AddDialog
{
    public static TimeEntryCreate? Add(int rowIndex)
    {
        var baseTask = Store.Instance.Tasks[rowIndex];
        var entryData = new TimeEntryCreate
        {
            UserId = Store.Instance.User.Id,
            TaskId = baseTask.Id,
            SubTaskId = null,
            Date = DateOnly.FromDateTime(DateTime.Now),
            Hours = Store.Instance.User.DefaultEntryHours,
            Comment = ""
        };

        TimeEntryCreate? result = null;

        var dialog = new Dialog()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Percent(40),
            Height = Dim.Percent(60),
            ShadowStyle = ShadowStyle.None,
            BorderStyle = LineStyle.Rounded,
            Title = "Add Entry",
        };


        var taskField = new TextField()
        {
            X = 1,
            Y = 1,
            TextAlignment = Alignment.Center,
            ReadOnly = true,
            Width = Dim.Fill(),
        };
        var projectData = Store.Instance.TaskToProject[entryData.TaskId];
        if (projectData != null) taskField.Text = projectData.Item1 + " - " + projectData.Item3;

        // Hours
        var hoursLabel = new Label()
        {
            X = 1,
            Y = Pos.Bottom(taskField) + 1,
            Title = "Hours:"
        };
        var hoursField = new TextValidateField()
        {
            X = Pos.Right(hoursLabel) + 1,
            Y = Pos.Top(hoursLabel),
            Width = Dim.Fill(2),
            Provider = new TextRegexProvider(@"^\d*([.,]?\d*)?$"),
            ColorScheme = Colors.ColorSchemes["InputField"]
        };

        // Move this out of constructor
        // This is a hack to get the default value to show up
        // doesnt like it when you set it in the constructork
        hoursField.Text = (entryData.Hours).ToString(CultureInfo.InvariantCulture);

        // Date
        var dateLabel = new Label()
        {
            X = 1,
            Y = Pos.Top(hoursLabel) + 2,
            Title = "Date:"
        };
        var dateField = new DateField()
        {
            X = Pos.Right(dateLabel) + 1,
            Y = Pos.Top(dateLabel),
            Width = Dim.Fill(2),
            Date = entryData.Date.ToDateTime(TimeOnly.MinValue),
            ColorScheme = Colors.ColorSchemes["InputField"]
        };

        // Comment
        var commentLabel = new Label()
        {
            X = 1,
            Y = Pos.Top(dateLabel) + 2,
            Title = "Comment:"
        };
        var commentField = new TextView()
        {
            X = 1,
            Y = Pos.Bottom(commentLabel),
            Width = Dim.Fill()! - 1,
            Height = Dim.Fill()! - 3,
            ColorScheme = Colors.ColorSchemes["InputField"],
            Text = entryData.Comment ?? "",
            WordWrap = true,
        };
        commentField.KeyDown += (s, e) =>
        {
            if (e == Key.Tab)
            {
                commentField.SuperView?.AdvanceFocus(NavigationDirection.Forward, TabBehavior.TabStop);
                e.Handled = true;
            }
            if (e == Key.Tab.WithShift)
            {
                commentField.SuperView?.AdvanceFocus(NavigationDirection.Backward, TabBehavior.TabStop);
                e.Handled = true;
            }
        };

        // Save button
        var saveButton = new Button()
        {
            Title = "Add",

        };
        saveButton.KeyDown += (s, e) =>
        {
            if (e == Key.Enter)
            {
                if (!float.TryParse(hoursField.Text.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var hours))
                    return;
                if (hours % 0.25 != 0) return;

                entryData.Hours = hours;
                entryData.Comment = commentField.Text.ToString();
                entryData.Date = DateOnly.FromDateTime(dateField.Date);

                result = entryData;
                Application.RequestStop();
            }
        };
        saveButton.MouseClick += (s, e) =>
        {
            if (!float.TryParse(hoursField.Text.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var hours))
                return;
            if (hours % 0.25 != 0) return;

            entryData.Hours = hours;
            entryData.Comment = commentField.Text.ToString();
            entryData.Date = DateOnly.FromDateTime(dateField.Date);

            result = entryData;
            Application.RequestStop();
        };

        dialog.Add(taskField, hoursLabel, hoursField, dateLabel, dateField, commentLabel, commentField);
        dialog.AddButton(saveButton);

        Application.Run(dialog);
        return result;
    }
}
