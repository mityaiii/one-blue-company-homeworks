using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20240923337600, TransactionBehavior.Default)]
public class UpdatedFieldsInTaskComments : Migration {
    public override void Up()
    {
        Alter.Table("task_comments")
            .AddColumn("modified_at").AsDateTime().Nullable()
            .AddColumn("deleted_at").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Column("modified_at")
            .FromTable("task_comments");
        
        Delete.Column("deleted_at")
            .FromTable("task_comments");
    }
}