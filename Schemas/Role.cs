namespace Backend.Core.Schemas;

public class Role : SchemaBase
{
    public required string Name { get; set; }

    public required int XeroAccountId { get; set; }
    public XeroAccount XeroAccount { get; set; } = null!;

    public ICollection<User> Users { get; set; } = null!;

    public bool IsInUse => Users.Count != 0;
}
