using Terminal.Gui;
public static class FilterDialog
{
    public static int Entries()
    {
        int result = 0;
        var d = new Dialog()
        {
            Width = Dim.Percent(40),
            Height = Dim.Percent(30),
            Title = "Filter Entries",
            ShadowStyle = ShadowStyle.None,
        };

        var minDate = new DateField()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill(),
            Date = Store.Instance.startDate.ToDateTime(TimeOnly.MinValue),
            ColorScheme = Colors.ColorSchemes["InputField"]
        };
        d.Add(minDate);

        var seperator = new Label() { X = 1, Y = Pos.Bottom(minDate) + 1, Text = "to" };
        d.Add(seperator);

        var maxDate = new DateField()
        {
            X = 1,
            Y = Pos.Bottom(seperator) + 1,
            Width = Dim.Fill(),
            Date = Store.Instance.endDate.ToDateTime(TimeOnly.MinValue),
            ColorScheme = Colors.ColorSchemes["InputField"]
        };
        d.Add(maxDate);


        var yes = new Button() { Text = "Yes" };
        yes.KeyDown += (s, k) =>
        {
            if (k == Key.Enter)
            {
                Store.Instance.startDate = DateOnly.FromDateTime(minDate.Date);
                Store.Instance.endDate = DateOnly.FromDateTime(maxDate.Date);
                result = 1;
                Application.RequestStop();

            }
        };
        yes.MouseClick += (s, k) =>
        {
            Store.Instance.startDate = DateOnly.FromDateTime(minDate.Date);
            Store.Instance.endDate = DateOnly.FromDateTime(maxDate.Date);
            result = 1;
            Application.RequestStop();
        };
        var no = new Button() { Text = "no" };
        no.KeyDown += (s, k) =>
        {
            if (k == Key.Enter)
            {
                Application.RequestStop();
            }
        };

        no.MouseClick += (s, k) =>
        {
            Application.RequestStop();

        };
        d.AddButton(yes);
        d.AddButton(no);

        Application.Run(d);
        return result;
    }
}
