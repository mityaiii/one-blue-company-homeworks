using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using WorkshopApp.Proto.Client;

namespace HomeworkApp.Services;

public class TaskCommentsService : WorkshopApp.Proto.Client.TaskCommentsService.TaskCommentsServiceBase
{
    private readonly ITaskCommentsService _taskCommentService;
    
    public TaskCommentsService(ITaskCommentsService taskCommentService)
    {
        _taskCommentService = taskCommentService;
    }
    
    public override async Task<V1GetTaskCommentResponse> V1GetTaskComment(V1GetTaskCommentRequest request, ServerCallContext context)
    {
        var taskComments = 
            await _taskCommentService.GetTaskComment(request.TaskId, request.IncludeDeleted, context.CancellationToken);

        var taskCommentsProto = taskComments
            .Select(tc => new V1TaskComment()
            {
                Id = tc.Id,
                TaskId = tc.TaskId,
                At = tc.At.ToTimestamp(),
                AuthorUserId = tc.AuthorUserId,
                Message = tc.Message,
                DeletedAt = tc.DeletedAt?.ToTimestamp(), 
                ModifiedAt = tc.ModifiedAt?.ToTimestamp()
            });

        
        return new V1GetTaskCommentResponse()
        {
            TaskComments = { taskCommentsProto }
        };
    }

    public override async Task<V1CreateTaskCommentResponse> V1CreateTaskComment(V1CreateTaskCommentRequest request,
        ServerCallContext context)
    {
        var taskCommentId = await _taskCommentService.CreateTaskComment(new CreateTaskCommentModel()
        {
            TaskId = request.TaskId,
            AuthorUserId = request.AuthorUserId,
            Message = request.Message,
            At = request.At.ToDateTimeOffset(),
        }, context.CancellationToken);
        
        return new V1CreateTaskCommentResponse()
        {
            TaskCommentId = taskCommentId
        };
    }

    public override async Task<Empty> V1UpdateTaskComment(V1UpdateTaskCommentRequest request,
        ServerCallContext context)
    {
        DateTimeOffset? at = null;
        if (request.DeletedAt is not null)
        {
            at = request.At.ToDateTimeOffset();
        }
        
        var updateTaskCommentModel = new UpdateTaskCommentModel()
        {
            Id = request.Id,
            TaskId = request.TaskId,
            AuthorUserId = request.AuthorUserId,
            At = at,
            Message = request.Message,
        };
        
        await _taskCommentService.UpdateTaskComment(updateTaskCommentModel, context.CancellationToken);

        return new Empty();
    }

    public override async Task<Empty> V1SetDeletedTaskComment(V1SetDeletedTaskCommentRequest request,
        ServerCallContext context)
    {
        await _taskCommentService.SetDeleted(request.TaskId, context.CancellationToken);

        return new Empty();
    }

}