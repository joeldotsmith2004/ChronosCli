namespace Backend.Core.Schemas;

public enum ContractType
{
    Unknown = 0,
    Casual = 1,
    Salary = 2,
}

public enum WorkLocation
{
    Unknown = 0,
    Office = 1,
    Remote = 2,
    ClientSite = 3,
}

[Flags]
public enum UserFeature
{
    None = 0,
    CanMultiInput = 1,
}


public class UserBase
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public ContractType ContractType { get; set; } = ContractType.Unknown;
    public float DefaultEntryHours { get; set; } = 8.0f;
    public bool IsAdmin { get; set; } = false;
    public bool IsProjectManager { get; set; } = false;
    public int features { get; set; } = 0;
}
