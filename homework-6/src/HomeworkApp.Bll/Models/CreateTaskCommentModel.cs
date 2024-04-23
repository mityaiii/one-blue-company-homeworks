namespace HomeworkApp.Bll.Models;

public class CreateTaskCommentModel
{
    public required long TaskId { get; init; }
    public required long AuthorUserId { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset At { get; init; }
}