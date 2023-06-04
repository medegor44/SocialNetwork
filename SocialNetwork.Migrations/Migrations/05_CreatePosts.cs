using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(5)]
public class CreatePosts : Migration 
{
    public override void Up()
    {
        Create.Table("Posts")
            .WithColumn("Id").AsCustom("BIGSERIAL").PrimaryKey()
            .WithColumn("UserId").AsInt64().ForeignKey("Users", "Id")
            .WithColumn("Text").AsString();
    }

    public override void Down()
    {
        Delete.Table("Posts");
    }
}