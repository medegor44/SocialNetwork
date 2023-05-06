using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(4)]
public class CreateFriends : Migration 
{
    private const string PkUseridFriendid = "pk_UserId_FriendId";

    public override void Up()
    {
        Create.Table("Friends")
            .WithColumn("UserId").AsInt64().ForeignKey("Users", "Id")
            .WithColumn("FriendId").AsInt64().ForeignKey("Users", "Id");
        Create.PrimaryKey(PkUseridFriendid)
            .OnTable("Friends")
            .Columns("UserId", "FriendId");
    }

    public override void Down()
    {
        Delete.PrimaryKey(PkUseridFriendid);
        Delete.Table("Friends");
    }
}