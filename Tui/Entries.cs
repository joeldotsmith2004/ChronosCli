using Terminal.Gui;
using System.Data;

public class Entries : Terminal.Gui.Window
{
    private const int gap = 1;

    public TableView entriesTable = null!;
    public DataTableSource entriesSource = null!;
    public DataTable entriesData = null!;

    public Entries()
    {
        this.X = 1;
        this.Y = 1;
        this.Width = Dim.Fill(gap);
        this.Height = Dim.Percent(60);
        this.BorderStyle = LineStyle.Rounded;
        this.Title = "1";

        entriesData = new DataTable();
        entriesData.Columns.Add("Entry Id", typeof(int));
        entriesData.Columns.Add("Task Id", typeof(int));
        entriesData.Columns.Add("Hours", typeof(float));
        entriesData.Columns.Add("Comment", typeof(string));

        entriesSource = new DataTableSource(entriesData);
        entriesTable = new TableView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            FullRowSelect = true,
            MultiSelect = false,
            Table = entriesSource,
            HotKey = '1',
            CollectionNavigator = null,

        };

        entriesTable.KeyBindings.Add(Key.H, Command.Left);
        entriesTable.KeyBindings.Add(Key.J, Command.Down);
        entriesTable.KeyBindings.Add(Key.K, Command.Up);
        entriesTable.KeyBindings.Add(Key.L, Command.Right);

        this.Add(entriesTable);
    }

}
