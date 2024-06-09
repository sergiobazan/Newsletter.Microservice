using Microsoft.EntityFrameworkCore;
using Newsletter.API.Database;
using Newsletter.API.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(configuration =>
    configuration.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddMigrations();
}

app.UseHttpsRedirection();

app.Run();
