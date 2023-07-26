using System.Data.SqlServerCe;
using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(7)]
public class Dialogs : Migration 
{
    public override void Up()
    {
        Create.Table("Dialogs")
            .WithColumn("From").AsInt64()
            .WithColumn("To").AsInt64()
            .WithColumn("CreateDate").AsDateTimeOffset()
            .WithColumn("Text").AsString();

        Execute.Sql(@"SELECT create_reference_table('""Cities""');");
        Execute.Sql(@"SELECT create_reference_table('""Users""');");
        Execute.Sql(@"SELECT create_distributed_table('""Dialogs""', 'From');");
    }

    public override void Down()
    {
        Delete.Table("Dialogs");
    }
}