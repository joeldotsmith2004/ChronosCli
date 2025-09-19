namespace Backend.Core.Schemas;
public class ProjectAssignedDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool RequiresCommentsOnTimeEntries { get; set; }
    public required bool CanAddEntries { get; set; }
    public required List<PurchaseOrderAssignedDto> PurchaseOrders { get; set; }
    public required bool IsManager { get; set; }
}

public class PurchaseOrderAssignedDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool CanAddEntries { get; set; }
    public required List<TaskAssignedDto> Tasks { get; set; }
}

public class TaskAssignedDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required bool CanAddEntries { get; set; }
    public required List<SubTaskAssignedDto> SubTasks { get; set; }
}

public class SubTaskAssignedDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
