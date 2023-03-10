using SocialNetwork.DataAccess.Extensions;
using SocialNetwork.Migrations;
using SocialNetwork.Postgres;
using SocialNetwork.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);
#if DEBUG
builder.Configuration.AddJsonFile("appsettings.Development.json");
#endif

builder.Services.AddConnectionStringBuilder();

if (args.Contains("migrate"))
{
    builder.Services
        .AddMigration(builder.Configuration);
    builder.Build()
        .Services
        .UpdateDatabase();
}
else
{
    builder.Services
        .AddPostgres()
        .AddRepositories()
        .AddHandlers()
        .AddLogging()
        .AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}