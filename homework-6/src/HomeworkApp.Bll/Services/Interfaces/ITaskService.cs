using HomeworkApp.Bll.Models;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;

namespace HomeworkApp.Bll.Services.Interfaces;

public interface ITaskService
{
    Task<long> CreateTask(CreateTaskModel model, CancellationToken token);

    Task<GetTaskModel?> GetTask(long taskId, CancellationToken token);

    Task AssignTask(Bll.Models.AssignTaskModel model, CancellationToken token);

    Task<GetSubTasksInStatusModel[]> GetSubTasksInStatus(long parentTaskId, 
        Enums.TaskStatus[] statuses,
        CancellationToken token);
    
    Task<TaskMessage[]> GetComments(long taskId, CancellationToken token);
}