﻿using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(1)]
public class CreateUsers : Migration 
{
    public override void Up()
    {
        Create.Table("Cities")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("Name").AsString(200);
        
        Create.Table("Users")
            .WithColumn("Id").AsGuid().PrimaryKey()
            .WithColumn("FirstName").AsString(200)
            .WithColumn("SecondName").AsString(200)
            .WithColumn("Age").AsInt32()
            .WithColumn("Biography").AsCustom("TEXT")
            .WithColumn("CityId").AsGuid().ForeignKey("Cities", "Id")
            .WithColumn("Password").AsString(200)
            .WithColumn("Salt").AsString(200);
    }

    public override void Down()
    {
        Delete.Table("Users");
        Delete.Table("Cities");
    }
}