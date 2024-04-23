using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;

namespace HomeworkApp.Dal.Repositories.Interfaces;

public interface ITaskCommentRepository
{
    Task<long> Add(TaskCommentEntityV1 model, CancellationToken token);
    Task Update(TaskCommentEntityV1 model, CancellationToken token);
    Task SetDeleted(long taskId, CancellationToken token);
    Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token);
    Task<TaskCommentEntityV1?> Get(long id, CancellationToken token);
    Task<TaskMessage[]> GetComments(long taskId, CancellationToken token);
}