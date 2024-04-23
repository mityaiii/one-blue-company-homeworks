namespace HomeworkApp.Bll.Models;

public class GetTaskCommentModel
{
    public required long Id { get; init; }
    public required long TaskId { get; init; }
    public required long AuthorUserId { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset At { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
    public DateTimeOffset? DeletedAt { get; init; }
}