using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Providers.Interfaces;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;
    public TaskCommentRepository(IOptions<DalOptions> dalSettings, IDateTimeProvider dateTimeProvider)
        : base(dalSettings.Value)
    {
        _dateTimeProvider = dateTimeProvider;
    }
    
    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
insert into task_comments (task_id, author_user_id, message, at)
values (@TaskId, @AuthorUserId, @Message, @At)
returning id;";

        await using var connection = await GetConnection();
        var id = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AuthorUserId = model.AuthorUserId,
                    Message = model.Message,
                    At = model.At
                },
                cancellationToken: token));

        return id.SingleOrDefault();
    }

    public async Task Update(TaskCommentEntityV1 query, CancellationToken token)
    {
        const string sqlQuery = @"update task_comments 
  set task_id = @TaskId
    , author_user_id = @AuthorUserId
    , message = @Message
    , at = @At
    , modified_at = @ModifiedAt
where id = 1;
";
        await using var connection = await GetConnection();
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = query.TaskId,
                    AuthorUserId = query.AuthorUserId,
                    Message = query.Message,
                    At = query.At,
                    ModifiedAt = _dateTimeProvider.Now()
                },
                cancellationToken: token));
    }

    public async Task SetDeleted(long taskId, CancellationToken token)
    {
        const string sqlQuery = @"
        update task_comments
           set deleted_at = @DeletedAt
         where task_id = @TaskId;
    ";

        await using var connection = await GetConnection();
        
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = taskId,
                    DeletedAt = _dateTimeProvider.Now()
                },
                cancellationToken: token));
    }

    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        var baseSql = @"
  select tc.id
       , tc.task_id
       , tc.author_user_id
       , tc.message
       , tc.at
       , tc.modified_at
       , tc.deleted_at
    from task_comments tc
   where tc.task_id = @TaskId
";
        if (!model.IncludeDeleted)
        {
            baseSql += " and deleted_at IS NULL";
        }
        baseSql += " order by tc.at desc;";
        
        await using var connection = await GetConnection();
        var taskComments = await connection.QueryAsync<TaskCommentEntityV1>(
            new CommandDefinition(baseSql,
                new
                {
                    TaskId = model.TaskId
                }, cancellationToken: token));

        return taskComments.ToArray();
    }

    public async Task<TaskCommentEntityV1?> Get(long id, CancellationToken token)
    {
        const string sqlQuery = @"
select tc.id
     , tc.task_id
     , tc.author_user_id
     , tc.message
     , tc.at
     , tc.modified_at
     , tc.deleted_at
from task_comments tc
where tc.id = @Id
  and deleted_at is null
";
        
        await using var connection = await GetConnection();
        var taskComment = await connection.QueryAsync<TaskCommentEntityV1>(
            new CommandDefinition(sqlQuery,
                new
                {
                    Id = id
                }, cancellationToken: token));

        return taskComment.SingleOrDefault();
    }

    public async Task<TaskMessage[]> GetComments(long taskId, CancellationToken token)
    {
        const string sqlQuery = @"
  select tc.task_id
       , tc.message
       , tc.deleted_at is null as is_deleted 
       , tc.at
   where task_id = @TaskId
    from task_comments tc
order by tc.at";

        await using var connection = await GetConnection();
        var taskMessages = await connection.QueryAsync<TaskMessage>(
            new CommandDefinition(sqlQuery,
                new
                {
                    TaskId = taskId
                }, cancellationToken: token));

        return taskMessages.ToArray();
    }
}