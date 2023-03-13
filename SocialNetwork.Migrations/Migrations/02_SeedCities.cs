using System.Text;
using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(2)]
public class SeedCities : Migration 
{
    private const string File = "../SocialNetwork.Migrations/Data/Cities.txt";

    public override void Up()
    {
        using var reader = new StreamReader(File);
        var values = new List<string>();
        
        while (!reader.EndOfStream)
            values.Add($"('{Guid.NewGuid()}', '{reader.ReadLine()}')");

        var insert = string.Join(",", values);
        
        var sql = $"""
INSERT INTO "Cities"("Id", "Name") VALUES
{insert}
""";
        
        Execute.Sql(sql);
    }

    public override void Down()
    {
    }
}