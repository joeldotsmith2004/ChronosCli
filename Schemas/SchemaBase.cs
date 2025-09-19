namespace Backend.Core.Schemas;

public abstract class SchemaBase
{
    public int Id { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

}
