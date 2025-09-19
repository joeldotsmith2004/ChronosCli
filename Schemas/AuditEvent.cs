namespace Backend.Core.Schemas;

public partial struct AuditEventId;

public class AuditEvent
{
    public int Id { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<AuditEntry> Entries { get; set; } = null!;
}
