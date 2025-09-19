
namespace Backend.Core.Schemas;

public class SubTask : SchemaBase
{
    public required string Name { get; set; }

    public required int TaskId { get; set; }
    public Task_ Task { get; set; } = null!;
}
