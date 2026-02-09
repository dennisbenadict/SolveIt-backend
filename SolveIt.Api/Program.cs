using MediatR;
using Microsoft.EntityFrameworkCore;
using SolveIt.Infrastructure.Persistence;
using Solvelt.Application.Interfaces;
using Solvelt.Application.Organizers.Commands.RegisterOrganizer;
using Solvelt.Infrastructure.Persistence;
using Solvelt.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<SolveItDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterOrganizerHandler).Assembly));

// Repositories
builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

