using Terminal.Gui;

public class BottomBar : StatusBar
{
    public Label userNameItem = null!;
    public Label emailItem = null!;
    public Label actionItem = null!;

    public BottomBar()
    {
        this.X = 0;
        this.Width = Dim.Fill();
        this.AlignmentModes = AlignmentModes.AddSpaceBetweenItems;

        userNameItem = new Label() { Text = "UserName" };
        emailItem = new Label() { Text = "Email" };
        actionItem = new Label() { Text = "" };
        this.Add(userNameItem);
        this.Add(emailItem);
        this.Add(actionItem);
    }
}
