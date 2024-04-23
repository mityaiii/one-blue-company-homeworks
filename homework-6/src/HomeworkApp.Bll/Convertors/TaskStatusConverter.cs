namespace HomeworkApp.Bll.Convertors;

public class TaskStatusConverter
{
    public Bll.Enums.TaskStatus Convert(Dal.Enums.TaskStatus dalStatus)
    {
        var bllStatus = dalStatus switch
        {
            Dal.Enums.TaskStatus.InProgress => Enums.TaskStatus.InProgress,
            Dal.Enums.TaskStatus.ToDo => Enums.TaskStatus.ToDo,
            Dal.Enums.TaskStatus.Draft => Enums.TaskStatus.Draft,
            Dal.Enums.TaskStatus.Canceled => Enums.TaskStatus.Canceled,
            Dal.Enums.TaskStatus.Done => Enums.TaskStatus.Done,
            _ => throw new ArgumentOutOfRangeException(nameof(dalStatus), dalStatus, null)
        };

        return bllStatus;
    }
    
    public Dal.Enums.TaskStatus Convert(Bll.Enums.TaskStatus bllStatus)
    {
        var dalStatus = bllStatus switch
        {
            Enums.TaskStatus.InProgress => Dal.Enums.TaskStatus.InProgress,
            Enums.TaskStatus.ToDo => Dal.Enums.TaskStatus.ToDo,
            Enums.TaskStatus.Draft => Dal.Enums.TaskStatus.Draft,
            Enums.TaskStatus.Canceled => Dal.Enums.TaskStatus.Canceled,
            Enums.TaskStatus.Done => Dal.Enums.TaskStatus.Done,
            _ => throw new ArgumentOutOfRangeException(nameof(bllStatus), bllStatus, null)
        };

        return dalStatus;
    }
}