using FluentAssertions;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentsRepositoryTests
{
    private readonly ITaskCommentRepository _repository;

    public TaskCommentsRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
    }

    [Fact]
    public async Task Add_TaskComment_Success()
    {
        // Arrange
        var tasks = TaskCommentEntityV1Faker.Generate();
        
        // Act
        var actualId = await _repository.Add(tasks.First(), default);

        // Asserts
        actualId.Should().BePositive();
    }

    [Fact]
    public async Task Get_TaskCommentWithTaskId_Success()
    {
        var taskId = Create.RandomId();
        // Arrange
        const int amount = 3;

        var expectedTaskComments = new TaskCommentEntityV1[2];
        var taskComments = TaskCommentEntityV1Faker.Generate(amount);
        taskComments[0] = taskComments[0]
            .WithTaskId(taskId);
        
        taskComments[1] = taskComments[1]
            .WithTaskId(taskId);

        taskComments[2] = taskComments[2]
            .WithTaskId(2);
        
        // Act
        expectedTaskComments[0] = taskComments[0]
            .WithId(await _repository.Add(taskComments[0], default));
        
        expectedTaskComments[1] = taskComments[1]
            .WithId(await _repository.Add(taskComments[1], default));
        
        taskComments[2]
            .WithId(await _repository.Add(taskComments[2], default));

        var actualTaskComments = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskId,
            IncludeDeleted = false
        }, default);

        actualTaskComments = actualTaskComments
            .OrderBy(x => x.Id)
            .ToArray();
        
        // Asserts
        actualTaskComments.Should().HaveCount(2);
        actualTaskComments.Should().BeEquivalentTo(expectedTaskComments);
    }
    
    [Fact]
    public async Task SetDeleted_TaskComment_Success()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.Generate();
        
        // Act
        await _repository.Add(taskComment.First(), default);
        await _repository.SetDeleted(taskComment.First().TaskId, default);
        var actualTaskComment = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskComment.First().TaskId,
            IncludeDeleted = true
        }, default);
        var actualDeletedAt = actualTaskComment.Single().DeletedAt;
        
        // Asserts
        actualDeletedAt.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Get_TaskCommentWithTaskIdWithDeletedTasks_Success()
    {
        // Arrange
        var taskId = Create.RandomId();
        const int amount = 3;

        var expectedTaskComments = new TaskCommentEntityV1[2];
        var taskComments = TaskCommentEntityV1Faker.Generate(amount);
        taskComments[0] = taskComments[0]
            .WithTaskId(taskId);
        
        taskComments[1] = taskComments[1]
            .WithTaskId(taskId);

        taskComments[2] = taskComments[2]
            .WithTaskId(2);
        
        // Act
        expectedTaskComments[0] = taskComments[0]
            .WithId(await _repository.Add(taskComments[0], default));
        
        expectedTaskComments[1] = taskComments[1]
            .WithId(await _repository.Add(taskComments[1], default));
        
        taskComments[2]
            .WithId(await _repository.Add(taskComments[2], default));

        await _repository.SetDeleted(taskComments[0].TaskId, default);
        
        var actualTaskComments = await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskId,
            IncludeDeleted = true
        }, default);

        actualTaskComments = actualTaskComments
            .OrderBy(x => x.Id)
            .ToArray();
        
        // Asserts
        actualTaskComments.Should().HaveCount(2)
            .And.OnlyContain(comment => comment.DeletedAt.HasValue);
    }
    
    [Fact]
    public async Task Update_TaskComment_Success()
    {
        // Arrange
        var taskId = Create.RandomId();
        var taskComment = TaskCommentEntityV1Faker.Generate().Single();

        // Act
        taskComment = taskComment.WithId(await _repository.Add(taskComment, default));
        await _repository.Update(new TaskCommentEntityV1()
        {
            TaskId = taskId,
            At = taskComment.At,
            Message = taskComment.Message,
            Id = taskComment.Id,
            AuthorUserId = taskComment.AuthorUserId
        }, default);

        var actualTaskComment = (await _repository.Get(new TaskCommentGetModel()
        {
            TaskId = taskId,
            IncludeDeleted = false
        }, default)).Single(); 
        
        // Assets
        actualTaskComment.ModifiedAt.Should().NotBeNull();
        actualTaskComment.TaskId.Should().Be(taskId);
    }
}