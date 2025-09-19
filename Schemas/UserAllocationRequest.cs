namespace Backend.Core.Schemas;

public enum RequestStatus
{
    Pending = 1,
    Accepted = 2,
    Rejected = 3,
}

public class UserAllocationRequest : SchemaBase
{
    public required int UserId { get; set; }
    public User User { get; set; } = null!;

    public required int? ProjectId { get; set; }
    public Project? Project { get; set; } = null!;

    public required int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; } = null!;

    public required int? TaskId { get; set; }
    public Task_? Task { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.Pending;
}
