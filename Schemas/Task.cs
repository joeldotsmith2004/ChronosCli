namespace Backend.Core.Schemas;

public enum TaskStatus
{
    Active = 1,
    Invoiced = 2,
    Locked = 3,
}

public enum ChargeType
{
    Time = 1,
    Fixed = 2,
    NonChargeable = 3,
}

// ReSharper disable once InconsistentNaming
public class Task_ : SchemaBase
{
    public required string Name { get; set; }

    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public TaskStatus Status { get; set; }
    public ChargeType ChargeType { get; set; }

    public int EstimateMinutes { get; set; }
    public int TotalMinutes { get; set; }

    public int DeltaMinutes { get; set; }

    public Guid XeroId { get; set; }
    public DateTimeOffset LastSyncedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? DeletedAt { get; set; }

    public bool IsOpen => !IsDeleted;

    public ICollection<TaskUser> TaskUsers { get; set; } = null!;
    public ICollection<SubTask> SubTasks { get; set; } = null!;

    public ICollection<TimeEntry> TimeEntries { get; set; } = null!;
}
