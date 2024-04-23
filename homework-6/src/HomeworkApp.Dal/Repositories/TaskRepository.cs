using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.Dal.Repositories;

public class TaskRepository : PgRepository, ITaskRepository
{
    public TaskRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long[]> Add(TaskEntityV1[] tasks, CancellationToken token)
    {
        const string sqlQuery = @"
insert into tasks (parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at) 
select parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at
  from UNNEST(@Tasks)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Tasks = tasks
                },
                cancellationToken: token));
        
        return ids
            .ToArray();
    }

    public async Task<TaskEntityV1[]> Get(TaskGetModel query, CancellationToken token)
    {
        var baseSql = @"
select id
     , parent_task_id
     , number
     , title
     , description
     , status
     , created_at
     , created_by_user_id
     , assigned_to_user_id
     , completed_at
  from tasks
";
        
        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (query.TaskIds.Any())
        {
            conditions.Add($"id = ANY(@TaskIds)");
            @params.Add($"TaskIds", query.TaskIds);
        }
        
        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskEntityV1>(cmd))
            .ToArray();
    }

    public async Task Assign(AssignTaskModel model, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
   set assigned_to_user_id = @AssignToUserId
     , status = @Status
 where id = @TaskId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AssignToUserId = model.AssignToUserId,
                    Status = model.Status
                },
                cancellationToken: token));
    }

    public async Task<SubTaskModel[]> GetSubTasksInStatus(long parentTaskId, TaskStatus[] statuses, CancellationToken token)
    {
        const string sqlQuery = @"
with recursive tasks_tree as (select t.id
                                   , t.title
                                   , t.status
                                   , array [t.parent_task_id, t.id] as arr
                              from tasks t
                              where t.parent_task_id = @TaskId
                              union all
                              select t1.id
                                   , t1.title
                                   , t1.status
                                   , arr || t1.id
                              from tasks t1
                                       join tasks_tree on tasks_tree.id = t1.parent_task_id)
select tt.id task_id
     , tt.title
     , tt.status
     , tt.arr parent_task_ids
from tasks_tree tt
where tt.status = any(@Statuses)
";

        var taskStatusesInt = statuses
            .Select(s => (int)s)
            .ToArray();
        
        await using var connection = await GetConnection();
        var subTasks = await connection.QueryAsync<SubTaskModel>(new CommandDefinition(
            sqlQuery,
            new { 
                TaskId = parentTaskId,
                Statuses = taskStatusesInt 
            },
            cancellationToken: token));
        
        return subTasks.ToArray();
    }

    public async Task SetParentTaskId(long? parentTaskId, long taskId, CancellationToken token)
    {
        const string sqlQuery = @"update tasks
  set parent_task_id = @ParentTaskId
where id = TaskId;
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    ParentTaskId = parentTaskId,
                    TaskIds = taskId
                },
                cancellationToken: token));
    }
}