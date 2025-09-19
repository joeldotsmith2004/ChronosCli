namespace Backend.Core.Schemas;

public class XeroAccount : SchemaBase
{
    public required string Name { get; set; }
    public required Guid XeroId { get; set; }

    public DateTimeOffset LastSyncedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
