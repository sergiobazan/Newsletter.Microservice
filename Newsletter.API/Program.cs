using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Newsletter.API.Database;
using Newsletter.API.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(configuration =>
    configuration.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(assembly));

builder.Services.AddCarter();

builder.Services.AddValidatorsFromAssembly(assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.AddMigrations();
}

app.MapCarter();

app.UseHttpsRedirection();

app.Run();
