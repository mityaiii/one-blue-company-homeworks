namespace HomeworkApp.Dal.Entities;

public record TaskCommentEntityV1
{
    public long Id { get; init; }
    public long TaskId { get; init; }
    public long AuthorUserId { get; init; }
    public string Message { get; init; }
    public DateTimeOffset At { get; init; }
    public DateTimeOffset? ModifiedAt { get; init; }
    public DateTimeOffset? DeletedAt { get; init; }
}