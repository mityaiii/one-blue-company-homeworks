namespace HomeworkApp.Bll.Models;

public record GetSubTasksInStatusModel
{
    public required long TaskId { get; init; }
    public required string Title { get; init; }
    public required Enums.TaskStatus Status { get; init; }
    public required long[] ParentTaskIds { get; init; }
}