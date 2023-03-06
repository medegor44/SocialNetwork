using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(1)]
public class CreateUsers : Migration 
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FirstName").AsCustom("VARCHAR(100)")
            .WithColumn("SecondName").AsCustom("VARCHAR(100)")
            .WithColumn("Age").AsInt32()
            .WithColumn("Biography").AsCustom("TEXT")
            .WithColumn("CityId").AsGuid();
    }

    public override void Down()
    {
        Delete.Table("Users");
    }
}