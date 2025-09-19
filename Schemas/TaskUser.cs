namespace Backend.Core.Schemas;

public class TaskUser : SchemaBase
{
    public required int UserId { get; set; }
    public User User { get; set; } = null!;

    public required int TaskId { get; set; }
    public Task_ Task { get; set; } = null!;

    public bool IsActive { get; set; } = true;
}
