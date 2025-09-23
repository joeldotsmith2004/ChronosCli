using Terminal.Gui;
using Terminal.Gui.TextValidateProviders;
using Backend.Core.Schemas;
using System.Globalization;

public class EntryDialog : Dialog
{
    public TextField taskField;
    public TextValidateField hoursField;
    public DateField dateField;
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
            Text = "Unknown",
            TextAlignment = Alignment.Center,
            ReadOnly = true,
            Width = Dim.Fill(),
        };
        if (data != null)
        {
            var projectData = Store.Instance.TaskToProject[data.TaskId];
            taskField.Text = projectData.Item1 + " - " + projectData.Item3;
        }

        // Hours
        var hoursLabel = new Label()
        {
            X = 1,
            Y = Pos.Bottom(taskField) + 1,
            Title = "Hours:"
        };
        hoursField = new TextValidateField()
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
        hoursField.Text = (data?.Hours ?? Store.Instance.User.DefaultEntryHours).ToString(CultureInfo.InvariantCulture);

        // Date
        var dateLabel = new Label()
        {
            X = 1,
            Y = Pos.Top(hoursLabel) + 2,
            Title = "Date:"
        };
        dateField = new DateField()
        {
            X = Pos.Right(dateLabel) + 1,
            Y = Pos.Top(dateLabel),
            Width = Dim.Fill(2),
            Date = DateTime.Now,
            ColorScheme = Colors.ColorSchemes["InputField"]
        };

        // Comment
        var commentLabel = new Label()
        {
            X = 1,
            Y = Pos.Top(dateLabel) + 2,
            Title = "Comment:"
        };
        commentField = new TextView()
        {
            X = 1,
            Y = Pos.Bottom(commentLabel),
            Width = Dim.Fill()! - 1,
            Height = Dim.Fill()!,
            ColorScheme = Colors.ColorSchemes["InputField"],
            Text = data?.Comment ?? "",
            WordWrap = true,
        };
        // commentField.KeyDown += (s, e) =>
        // {
        //     if (e == Key.Tab)
        //     {
        //         e.Handled = true;
        //         Application.Top?.InvokeCommand(Command.NextTabGroup);
        //     }
        // };

        // Save button
        AddButton(new Button()
        {
            Title = "Save"
        });

        Add(taskField, hoursLabel, hoursField, dateLabel, dateField, commentLabel, commentField);
    }
}
