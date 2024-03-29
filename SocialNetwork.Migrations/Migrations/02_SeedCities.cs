﻿using System.Text;
using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(2)]
public class SeedCities : Migration 
{
#if DEBUG
    private const string File = "../SocialNetwork.Migrations/Data/Cities.txt";
#else
    private const string File = "./Data/Cities.txt";
#endif

    public override void Up()
    {
        using var reader = new StreamReader(File);
        var values = new List<string>();
        
        while (!reader.EndOfStream)
            values.Add($"('{reader.ReadLine()}')");

        var insert = string.Join(",", values);
        
        var sql = $"""
INSERT INTO "Cities"("Name") VALUES
{insert}
""";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
    }
}