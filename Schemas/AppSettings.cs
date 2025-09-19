namespace Backend.Core.Schemas;

public class AppSettings : SchemaBase
{
    public required DateOnly TimesheetCutOffDate { get; set; }
}
