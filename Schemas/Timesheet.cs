namespace Backend.Core.Schemas;

public class Timesheet : SchemaBase
{
    public required int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public required DateOnly EndDate { get; set; }
    public required string FileName { get; set; }

    public required bool AwaitingSignature { get; set; } = false;

    public bool HasBeenUploaded { get; set; } = false;

    public ICollection<TimeEntry> TimeEntries { get; set; } = null!;
}
