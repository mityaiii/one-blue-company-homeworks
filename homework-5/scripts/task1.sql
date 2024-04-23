WITH in_progress_time AS (
    SELECT tl.task_id
         , tl.at as in_progress_at
      FROM task_logs tl
     WHERE tl.status = 3 /* In progress */)
   SELECT t.number task_number
          , t.title task_title
          , (SELECT ts.name
               FROM task_statuses ts
              WHERE ts.id = 4) status_name
          , (SELECT u.email
               FROM users u
              WHERE t.created_by_user_id = u.id) author_email
          , (SELECT u.email
               FROM users u
              WHERE t.assigned_to_user_id = u.id) assignee_email
          , t.created_at
          , ipt.in_progress_at,in_progress_at
          , t.completed_at
          , (t.completed_at - ipt.in_progress_at) as duration
     FROM tasks t
LEFT JOIN in_progress_time ipt ON t.id = ipt.task_id
    WHERE t.completed_at IS NOT NULL
      AND t.id NOT IN (SELECT tl.id
                         FROM task_logs tl
                        WHERE tl.status = 5 /* Is Cancelled */)
ORDER BY duration DESC
LIMIT 100;