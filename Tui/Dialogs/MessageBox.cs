using Terminal.Gui;
public static class MessageBox
{
    public static int YesNo(string title, string message)
    {
        var d = new Dialog()
        {
            Width = Dim.Percent(40),
            Height = Dim.Percent(30),
            Title = title,
            ShadowStyle = ShadowStyle.None,
        };

        var lbl = new Label() { X = Pos.Center(), Y = 1, Text = message };
        d.Add(lbl);

        var result = -1;
        var yes = new Button() { Text = "Yes" };
        yes.KeyDown += (s, k) =>
        {
            if (k == Key.Enter)
            {
                result = 0;
                Application.RequestStop();

            }
        };
        yes.MouseClick += (s, k) =>
        {
            result = 0;
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
