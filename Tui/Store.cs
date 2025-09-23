using Backend.Core.Schemas;
public sealed class Store
{
    private static readonly Lazy<Store> _instance = new(() => new Store());
    public static Store Instance => _instance.Value;

    private Store() { }

    public UserBase User { get; set; } = null!;
    public List<TimeEntryGet> Entries { get; set; } = new();
    public List<ProjectAssignedDto> Tasks { get; set; } = new();

    // TaskId, <ProjectName, PurchaseOrderName, TaskName>
    public Dictionary<int, Tuple<string, string, string>> TaskToProject { get; set; } = new();

    // public DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-(7 + (int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Monday) % 7));
    // public DateOnly endDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(7 - (int)DateTime.Today.DayOfWeek % 7));
    public DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-14));
    public DateOnly endDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
}
