
namespace Backend.Core.Schemas;

// Leave type enum
public enum LeaveType
{
    Annual = 1,
    Sick = 2,
    Unpaid = 3,
    PublicHoliday = 4,
    // Other = 4,
}

public class LeaveEntry : SchemaBase
{
    public required int UserId { get; set; }
    public User User { get; set; } = null!;

    public required DateOnly Date { get; set; }

    public required float Hours { get; set; }

    public required LeaveType Type { get; set; }

    public string? Comment { get; set; }

    public int? PayRunId { get; set; }
    public PayRun? PayRun { get; set; }

    public bool BeenPaid => PayRunId != null;

    public bool IsEditable => !BeenPaid;
}
