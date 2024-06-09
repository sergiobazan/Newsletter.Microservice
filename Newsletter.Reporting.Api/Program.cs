using Microsoft.EntityFrameworkCore;
using Carter;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Extentions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssembly(assembly));

builder.Services.AddCarter();

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
