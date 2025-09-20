namespace Backend.Core.Schemas;

public class TimeEntry : SchemaBase
{
    public required int UserId { get; set; }
    public User User { get; set; } = null!;

    public required int TaskId { get; set; }
    public Task_ Task { get; set; } = null!;

    public required int? SubTaskId { get; set; }
    public SubTask? SubTask { get; set; }

    public required DateOnly Date { get; set; }

    public required float Hours { get; set; }
    public required string? Comment { get; set; }

    public int Minutes => (int)(Hours * 60);

    public int? TimesheetId { get; set; }
    public Timesheet? Timesheet { get; set; }

    public bool HasTimesheet => TimesheetId != null;

    public int? PayRunId { get; set; }
    public PayRun? PayRun { get; set; }

    public bool HasPayRun => PayRunId != null;

    public bool IsEditable => !HasTimesheet && !HasPayRun;
}

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


