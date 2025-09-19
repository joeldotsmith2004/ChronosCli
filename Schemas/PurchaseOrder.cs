namespace Backend.Core.Schemas;

public class PurchaseOrder : SchemaBase
{
    public required string Name { get; set; }

    public string? Number { get; set; }

    public int? ProjectId { get; set; }
    public Project? Project { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public bool IsOpen { get; set; } = true;

    public string? Comment { get; set; }

    public int TimesheetDay { get; set; } = 31;

    public string? SupervisorName { get; set; }
    public string? SupervisorEmail { get; set; }

    public bool RequiresSignedInvoices { get; set; }

    public Guid XeroId { get; set; }
    public DateTimeOffset LastSyncedAt { get; set; }

    public int InvoicedPercentage { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public ICollection<Task_> Tasks { get; set; } = null!;

    public ICollection<Timesheet> Timesheets { get; set; } = null!;
}
