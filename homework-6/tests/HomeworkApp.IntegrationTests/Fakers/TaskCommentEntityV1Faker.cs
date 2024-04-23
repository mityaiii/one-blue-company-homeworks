using AutoBogus;
using Bogus;
using HomeworkApp.Dal.Entities;
using HomeworkApp.IntegrationTests.Creators;

namespace HomeworkApp.IntegrationTests.Fakers;

public static class TaskCommentEntityV1Faker
{
    private static readonly object Lock = new();

    private static readonly Faker<TaskCommentEntityV1> Faker = new AutoFaker<TaskCommentEntityV1>()
        .RuleFor(x => x.Id, _ => Create.RandomId())
        .RuleFor(x => x.TaskId, f => Create.RandomId())
        .RuleFor(x => x.Message, f => f.Lorem.Locale)
        .RuleFor(x => x.At, f => f.Date.RecentOffset().UtcDateTime)
        .RuleFor(x => x.AuthorUserId, f => Create.RandomId())
        .RuleFor(x => x.DeletedAt, x => null)
        .RuleFor(x => x.ModifiedAt, x => null)
        .RuleForType(typeof(long), f => f.Random.Long(0L));   
    
    public static TaskCommentEntityV1[] Generate(int count = 1)
    {
        lock (Lock)
        {
            return Faker.Generate(count).ToArray();
        }
    }
    
    public static TaskCommentEntityV1 WithDeletedAt(
        this TaskCommentEntityV1 src, 
        DateTimeOffset dateTimeOffset)
        => src with { DeletedAt = dateTimeOffset };
    
    public static TaskCommentEntityV1 WithModifiedAt(
        this TaskCommentEntityV1 src, 
        DateTimeOffset modifiedAt)
        => src with { ModifiedAt = modifiedAt };
    public static TaskCommentEntityV1 WithTaskId(
        this TaskCommentEntityV1 src,
        long taskId) => src with { TaskId = taskId};

    public static TaskCommentEntityV1 WithId(this TaskCommentEntityV1 src,
        long id) => src with { Id = id };
}