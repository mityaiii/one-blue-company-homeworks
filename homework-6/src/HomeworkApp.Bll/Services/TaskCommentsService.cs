using System.Text.Json;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;

namespace HomeworkApp.Bll.Services;

public class TaskCommentsService : ITaskCommentsService
{
    private readonly ITaskCommentRepository _taskCommentRepository;

    public TaskCommentsService(ITaskCommentRepository taskCommentRepository)
    {
        _taskCommentRepository = taskCommentRepository;
    }
    
    public async Task<long> CreateTaskComment(CreateTaskCommentModel model, CancellationToken token)
    {
        var createTaskEntity = new TaskCommentEntityV1()
        {
            TaskId = model.TaskId,
            At = model.At,
            AuthorUserId = model.AuthorUserId,
            Message = model.Message
        };
        
        var result = await _taskCommentRepository.Add(createTaskEntity, token);

        return result;
    }
    
    public async Task<GetTaskCommentModel[]?> GetTaskComment(long taskId, bool includeDeleted, CancellationToken token)
    {
        var taskComments = await _taskCommentRepository.Get(new TaskCommentGetModel()
        {
            TaskId = taskId,
            IncludeDeleted = includeDeleted
        }, token);

        var result = taskComments
            .Select(tc => new GetTaskCommentModel()
            {
                Id = tc.Id,
                TaskId = tc.TaskId,
                At = tc.At,
                Message = tc.Message,
                AuthorUserId = tc.AuthorUserId,
                ModifiedAt = tc.ModifiedAt,
                DeletedAt = tc.DeletedAt
            })
            .ToArray();
        
        return result;
    }

    public async Task SetDeleted(long taskId, CancellationToken token)
    {
        await _taskCommentRepository.SetDeleted(taskId, token);
    }

    public async Task UpdateTaskComment(UpdateTaskCommentModel model, CancellationToken token)
    {
        var taskComment = await _taskCommentRepository.Get(model.Id, token);
        if (taskComment is null)
        {
            return;
        }

        await _taskCommentRepository.Update( new TaskCommentEntityV1()
        {
            Id = model.Id,
            TaskId = model.TaskId ?? taskComment.TaskId,
            AuthorUserId = model.AuthorUserId ?? taskComment.AuthorUserId,
            Message = model.Message ?? taskComment.Message,
            At = model.At ?? taskComment.At
        }, token);
    }
}