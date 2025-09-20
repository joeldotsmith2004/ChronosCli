namespace Backend.Core.Schemas;

public class TimeEntryGet
{
    public int Id { get; set; }
    public required int TaskId { get; set; }
    public required int PurchaseOrderId { get; set; }
    public required int? ProjectId { get; set; }
    public required int? SubTaskId { get; set; }
    public required DateOnly Date { get; set; }
    public required float Hours { get; set; }
    public required string? Comment { get; set; }
    public required bool IsEditable { get; set; }
}

public class TimeEntryCreate
{
    public required int UserId { get; set; }
    public required int TaskId { get; set; }
    public required int? SubTaskId { get; set; }
    public required DateOnly Date { get; set; }
    public required float Hours { get; set; }
    public required string? Comment { get; set; }
}


