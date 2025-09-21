using Terminal.Gui;
using Backend.Core.Schemas;

public class EntryDialog : Dialog
{
    public TextField taskField;
    public TextField hoursField;
    public TextField dateField;
    public TextView commentField;

    public EntryDialog(TimeEntryGet? data)
    {
        X = Pos.Center();
        Y = Pos.Center();
        Width = Dim.Percent(40);
        Height = Dim.Percent(60);
        ShadowStyle = ShadowStyle.None;
        BorderStyle = LineStyle.Rounded;
        Title = data == null ? "New Entry" : "Edit Entry";

        taskField = new TextField()
        {
            X = 1,
            Y = 1,
            Text = data?.TaskId.ToString() ?? "Task Unknown?",
            TextAlignment = Alignment.Center,
            ReadOnly = true,
            Width = Dim.Fill(),
        };

        // Hours
        var hoursLabel = new Label()
        {
            X = 1,
            Y = Pos.Bottom(taskField) + 1,
            Title = "Hours:"
        };

        hoursField = new TextField()
        {
            X = Pos.Right(hoursLabel) + 1,
            Y = Pos.Top(hoursLabel),
            Width = 10,
            ColorScheme = Colors.ColorSchemes["Highlighted"]
        };

        // Date
        var dateLabel = new Label()
        {
            X = Pos.Right(hoursField) + 2,
            Y = Pos.Top(hoursLabel),
            Title = "Date:"

        };

        dateField = new TextField()
        {
            X = Pos.Right(dateLabel) + 1,
            Y = Pos.Top(dateLabel),
            Width = 12,
            Title = DateTime.Now.ToShortDateString(),
            ColorScheme = Colors.ColorSchemes["Highlighted"]
        };

        // Comment
        var commentWindow = new Window()
        {
            X = 1,
            Y = Pos.Bottom(hoursLabel) + 1,
            Width = Dim.Fill(2),
            Height = Dim.Fill(2),
            Title = "Comment:",
            BorderStyle = LineStyle.Rounded
        };

        commentField = new TextView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = Colors.ColorSchemes["Highlighted"]
        };
        commentWindow.Add(commentField);

        // Save button
        AddButton(new Button()
        {
            X = Pos.Center(),
            Y = Pos.Bottom(commentWindow) + 1,
            Title = "Save"
        });

        Add(taskField, hoursLabel, hoursField, dateLabel, dateField, commentWindow);
    }
}
