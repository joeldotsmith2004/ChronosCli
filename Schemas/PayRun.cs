namespace Backend.Core.Schemas;

public class PayRun : SchemaBase
{
    public required DateOnly EndDate { get; set; }

    public ICollection<PayRunUser> PayRunUsers { get; set; } = null!;

    public ICollection<TimeEntry> TimeEntries { get; set; } = null!;
    public ICollection<LeaveEntry> LeaveEntries { get; set; } = null!;
}

public class PayRunUser : SchemaBase
{
    public int PayRunId { get; set; }
    public PayRun PayRun { get; set; } = null!;

    public required int UserId { get; set; }
    public User User { get; set; } = null!;

    public required ContractType ContractType { get; set; }
}
