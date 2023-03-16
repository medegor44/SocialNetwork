using Microsoft.AspNetCore.Authentication.JwtBearer;
using SocialNetwork;
using SocialNetwork.DataAccess.Extensions;
using SocialNetwork.Middleware;
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
        .AddPasswordHashingService()
        .AddLogging()
        .AddControllers();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = AuthOptions.Audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey()
            };
        });
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<BadStatusCodesHandlingMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}