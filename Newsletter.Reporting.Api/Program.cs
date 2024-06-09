using Microsoft.EntityFrameworkCore;
using Carter;
using Newsletter.Reporting.Api.Database;
using Newsletter.Reporting.Api.Extentions;
using MassTransit;
using Newsletter.Reporting.Api.Features.Articles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Database")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(conf => conf.RegisterServicesFromAssembly(assembly));

builder.Services.AddCarter();

builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();

    config.AddConsumer<ArticleCreated>();
    config.AddConsumer<ArticleViewed>();

    config.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.ConfigureEndpoints(context);
    });
});

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
