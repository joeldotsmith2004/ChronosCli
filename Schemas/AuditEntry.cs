namespace Backend.Core.Schemas;

public enum AuditType
{
    Create = 1,
    Update = 2,
    Delete = 3,
    SoftDelete = 4,
    Restore = 5,
}

public class AuditEntry
{
    public int Id { get; set; }

    public string TableName { get; set; } = null!;
    public AuditType Type { get; set; }

    public int EventId { get; set; }
    public AuditEvent Event { get; set; } = null!;

    public required int RecordId { get; set; }

    public required Dictionary<string, object?> Changes { get; set; }
}
