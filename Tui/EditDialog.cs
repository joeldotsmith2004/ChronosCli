using Terminal.Gui;
using Terminal.Gui.TextValidateProviders;
using Backend.Core.Schemas;
using System.Globalization;

public static class EditDialog
{
    public static TimeEntryGet? Edit(int rowIndex)
    {
        var original = Store.Instance.Entries[rowIndex];
        var entryData = new TimeEntryGet
        {
            Id = original.Id,
            PurchaseOrderId = original.PurchaseOrderId,
            ProjectId = original.ProjectId,
            SubTaskId = original.SubTaskId,
            IsEditable = original.IsEditable,
            TaskId = original.TaskId,
            Hours = original.Hours,
            Comment = original.Comment,
            Date = original.Date
        };

        TimeEntryGet? result = null;

        var dialog = new Dialog()
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = Dim.Percent(40),
            Height = Dim.Percent(60),
            ShadowStyle = ShadowStyle.None,
            BorderStyle = LineStyle.Rounded,
            Title = "Edit Entry",
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

        var hoursLabel = new Label() { X = 1, Y = Pos.Bottom(taskField) + 1, Text = "Hours:" };
        var hoursField = new TextValidateField()
        {
            X = Pos.Right(hoursLabel) + 1,
            Y = Pos.Top(hoursLabel),
            Width = Dim.Fill(2),
            Provider = new TextRegexProvider(@"^\d*([.,]?\d*)?$"),
            ColorScheme = Colors.ColorSchemes["InputField"],
            Text = entryData.Hours.ToString(CultureInfo.InvariantCulture)
        };

        var dateLabel = new Label() { X = 1, Y = Pos.Top(hoursLabel) + 2, Text = "Date:" };
        var dateField = new DateField()
        {
            X = Pos.Right(dateLabel) + 1,
            Y = Pos.Top(dateLabel),
            Width = Dim.Fill(2),
            Date = entryData.Date.ToDateTime(TimeOnly.MinValue),
            ColorScheme = Colors.ColorSchemes["InputField"]
        };

        var commentLabel = new Label() { X = 1, Y = Pos.Top(dateLabel) + 2, Text = "Comment:" };
        var commentField = new TextView()
        {
            X = 1,
            Y = Pos.Bottom(commentLabel),
            Width = Dim.Fill()! - 1,
            Height = Dim.Fill()! - 3,
            ColorScheme = Colors.ColorSchemes["InputField"],
            Text = entryData.Comment ?? "",
            WordWrap = true
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

        var saveButton = new Button() { Text = "Save" };
        saveButton.KeyDown += (s, k) =>
        {
            if (k == Key.Enter)
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
        saveButton.MouseClick += (s, k) =>
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
