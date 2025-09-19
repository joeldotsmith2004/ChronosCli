namespace Backend.Core.Schemas;

public class Client : SchemaBase
{
    public required string Name { get; set; }

    public Guid XeroId { get; set; }
    public DateTimeOffset LastSyncedAt { get; set; }
}
