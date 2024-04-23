using FluentAssertions;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);
        
        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }
    
    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);
        
        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }
    
    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();
        
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);
        
        // Act
        await _repository.Assign(assign, default);
        
        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        results.Should().HaveCount(1);
        var task = results.Single();
        
        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task GetSubTasksInStatus_Success()
    {
        const int count = 5;
        var tasks = TaskEntityV1Faker.Generate(count);
        var expectedTaskStatuses = new[] { TaskStatus.InProgress, TaskStatus.Done };
        
        tasks[0] = tasks[0].WithParentId(null)
            .WithStatus(TaskStatus.InProgress)
            .WithId(1);
        
        tasks[1] = tasks[1].WithParentId(tasks[0].Id)
            .WithStatus(TaskStatus.ToDo)
            .WithId(2);
        
        tasks[2] = tasks[2].WithParentId(tasks[0].Id)
            .WithStatus(TaskStatus.Done)
            .WithId(3);
        
        tasks[3] = tasks[3].WithParentId(tasks[2].Id)
            .WithStatus(TaskStatus.Done)
            .WithId(4);

        tasks[4] = tasks[4].WithParentId(null)
            .WithStatus(TaskStatus.Done)
            .WithId(5);

        var ids = await _repository.Add(tasks, default);
        var expectedSubTasksInStatus = new SubTaskModel[]
        {
            new SubTaskModel()
            {
                TaskId = tasks[2].Id,
                ParentTaskIds = [tasks[0].Id, tasks[2].Id],
                Status = TaskStatus.Done,
                Title = tasks[2].Title
            },
            new SubTaskModel()
            {
                TaskId = tasks[3].Id,
                ParentTaskIds = [tasks[0].Id, tasks[2].Id, tasks[3].Id],
                Status = TaskStatus.Done,
                Title = tasks[3].Title
            }
        };
        
        // Act
        var actualTasks = await _repository.GetSubTasksInStatus(tasks[0].Id, expectedTaskStatuses, default);
        
        // Assert
        actualTasks.Should().BeEquivalentTo(actualTasks);
    }
}
