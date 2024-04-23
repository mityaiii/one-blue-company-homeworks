using HomeworkApp.Bll.Models;

namespace HomeworkApp.Bll.Services.Interfaces;

public interface ITaskCommentsService
{
    Task<long> CreateTaskComment(CreateTaskCommentModel model, CancellationToken token);
    Task<GetTaskCommentModel[]?> GetTaskComment(long taskId, bool includeDeleted, CancellationToken token);
    Task SetDeleted(long taskId, CancellationToken token);
    Task UpdateTaskComment(UpdateTaskCommentModel model, CancellationToken token);
}