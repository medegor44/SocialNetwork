using FluentMigrator;

namespace SocialNetwork.Migrations.Migrations;

[Migration(3)]
public class SeedUsers : Migration
{
#if DEBUG
    private const string File = "../SocialNetwork.Migrations/Data/People.csv";
#else
    private const string File = "./Data/People.csv";
#endif

    public IEnumerable<string> Read()
    {
        using var reader = new StreamReader(File);

        while (!reader.EndOfStream)
        {
            var strings = reader.ReadLine().Split(",", StringSplitOptions.RemoveEmptyEntries);

            var names = strings[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var firstName = names[1];
            var secondName = names[0];
            var age = strings[1];
            var city = strings[2];
            
            yield return ($"('{Guid.NewGuid()}'::UUID, '{firstName}', '{secondName}', {age}, '', '{city}', '', '')");
        }
    }

    public override void Up()
    {
        var list = Read().ToList();
        var enumLines = list.AsEnumerable();

        for (int i = 0; i < list.Count; i += 100)
        {
            var values = enumLines.Take(100);
            enumLines = enumLines.Skip(100);
            
            
            var sql = $"""
INSERT INTO "Users"("Id", "FirstName", "SecondName", "Age", "Biography", "CityId", "Password", "Salt")
(
    SELECT
        t."Id",
        t."FirstName",
        t."SecondName",
        t."Age",
        t."Biography",
        c."Id" AS "CityId",
        t."Password",
        t."Salt"
    FROM (VALUES {string.Join($",{Environment.NewLine}", values)}) AS t ("Id", "FirstName", "SecondName", "Age", "Biography", "City", "Password", "Salt")
        JOIN "Cities" c ON t."City" = c."Name"
)
""";
            Execute.Sql(sql);
        }

    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}