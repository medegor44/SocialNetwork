using System.Data.SqlServerCe;
using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(7)]
public class Dialogs : Migration 
{
    public override void Up()
    {
        Create.Table("Dialogs")
            .WithColumn("Id").AsCustom("BIGSERIAL").PrimaryKey()
            .WithColumn("From").AsInt64().ForeignKey("Users", "Id")
            .WithColumn("To").AsInt64().ForeignKey("Users", "Id")
            .WithColumn("CreateDate").AsDateTimeOffset()
            .WithColumn("Text").AsString();
        
        Execute.Sql("SELECT create_distributed_table('Dialogs', 'From');");
    }

    public override void Down()
    {
        Delete.Table("Dialogs");
    }
}