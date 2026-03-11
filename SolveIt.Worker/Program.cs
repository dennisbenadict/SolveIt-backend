using MassTransit;
using Microsoft.EntityFrameworkCore;
using SolveIt.Infrastructure.Persistence;
using SolveIt.Infrastructure.Repositories;
using SolveIt.Worker.Consumers;
using SolveIt.Application.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDbContext<SolveItDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<ITestCaseRepository, TestCaseRepository>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<SubmissionConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var host = builder.Build();

await host.RunAsync();