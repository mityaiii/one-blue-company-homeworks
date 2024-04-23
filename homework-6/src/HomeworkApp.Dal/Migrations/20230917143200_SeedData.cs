using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20230917143200, TransactionBehavior.None)]
public class SeedData : Migration
{
    public override void Up()
    {
        const string task_statuses = @"
insert into task_statuses (id, alias, name, description)
values (1,      'Draft',        'Черновик', 'Задание не готово к передаче на выполнение')
     , (2,       'ToDo', 'Готово к работе', 'Задание может быть назначено на пользователя')
     , (3, 'InProgress',        'В работе', 'Задание назначено на исполнителя и находится в работе')
     , (4,       'Done',       'Выполнено', 'Работа над заданием завершена')
     , (5,   'Canceled',        'Отменено', 'Задание отклонено автором');
";
        
        Execute.Sql(task_statuses);
    }

    public override void Down()
    {
        Delete.FromTable("task_statuses")
            .AllRows();
    }
}