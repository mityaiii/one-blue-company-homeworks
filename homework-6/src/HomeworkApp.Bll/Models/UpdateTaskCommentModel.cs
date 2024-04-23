namespace HomeworkApp.Bll.Models;

public class UpdateTaskCommentModel
{
    public required long Id { get; init; }
    public long? TaskId { get; init; }
    public long? AuthorUserId { get; init; }
    public string? Message { get; init; }
    public DateTimeOffset? At { get; init; }
}