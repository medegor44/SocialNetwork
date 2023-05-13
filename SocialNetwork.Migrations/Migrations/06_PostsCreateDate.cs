using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(6)]
public class PostsCreateDate : Migration 
{
    public override void Up()
    {
        Alter.Table("Posts").AddColumn("CreateDate").AsDateTimeOffset();
    }

    public override void Down()
    {
        Delete.Column("CreateDate").FromTable("Posts");
    }
}