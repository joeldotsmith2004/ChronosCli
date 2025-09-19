namespace Backend.Core.Schemas;

public class Project : SchemaBase
{
    public string? Name { get; set; }

    public bool RequiresCommentsOnTimeEntries { get; set; }
    public bool CanAddEntries { get; set; }

    public string? Comment { get; set; }

    public List<ProjectUser> ProjectUsers { get; set; } = null!;
    public List<PurchaseOrder> PurchaseOrders { get; set; } = null!;
}
