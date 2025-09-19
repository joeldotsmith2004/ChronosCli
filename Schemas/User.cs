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

public class User : SchemaBase
{
    public required string Name { get; set; }

    public required string Email { get; set; }
    public required string MicrosoftId { get; set; }

    public required string? JobTitle { get; set; }

    public float DefaultEntryHours { get; set; } = 8.0f;

    public bool IsAdmin { get; set; } = false;

    public ContractType ContractType { get; set; } = ContractType.Unknown;
    public WorkLocation PrimaryWorkLocation { get; set; } = WorkLocation.Unknown;

    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    public bool IsActive { get; set; } = true;

    public UserFeature Features { get; set; } = UserFeature.None;

    public bool HasFeature(UserFeature feature) => Features.HasFlag(feature);

    public bool AccountSetupComplete => ContractType != ContractType.Unknown && RoleId != null;

    public ICollection<TimeEntry> TimeEntries { get; set; } = null!;
    public ICollection<LeaveEntry> LeaveEntries { get; set; } = null!;
}
