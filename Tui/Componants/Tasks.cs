using Terminal.Gui;
using System.Data;

public class Tasks : Window
{
    private const int gap = 1;

    public TableView taskTable = null!;
    public DataTableSource taskSource = null!;
    public DataTable taskData = null!;


    public Tasks(View UnderView)
    {
        this.X = gap;
        this.Y = Pos.Bottom(UnderView);
        this.Width = Dim.Fill(gap);
        this.BorderStyle = LineStyle.Rounded;
        this.Height = Dim.Fill(gap);
        this.Title = "2";

        taskData = new DataTable();
        taskData.Columns.Add("Project", typeof(string));
        taskData.Columns.Add("Task Id", typeof(int));
        taskData.Columns.Add("Name", typeof(string));
        taskData.Columns.Add("Purchase Order", typeof(string));

        taskSource = new DataTableSource(taskData);
        taskTable = new TableView()
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            FullRowSelect = true,
            MultiSelect = false,
            Table = taskSource,
            HotKey = '2',
            CollectionNavigator = null,
        };


        taskTable.KeyBindings.Add(Key.H, Command.Left);
        taskTable.KeyBindings.Add(Key.J, Command.Down);
        taskTable.KeyBindings.Add(Key.K, Command.Up);
        taskTable.KeyBindings.Add(Key.L, Command.Right);

        this.Add(taskTable);
    }
}
