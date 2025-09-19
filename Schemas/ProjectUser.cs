
namespace Backend.Core.Schemas;

public class ProjectUser : SchemaBase
{
    public required int UserId { get; set; }
    public User User { get; set; } = null!;

    public required int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    public bool IsManager { get; set; } = false;
    // public bool IsFavourite { get; set; } = false;
}
