//using FluentValidation;
//using FluentValidation.AspNetCore;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using SolveIt.Infrastructure.Persistence;
//using Solvelt.Api.Middleware;
//using Solvelt.Application.Interfaces;
//using Solvelt.Application.Organizers.Commands.RegisterOrganizer;
//using Solvelt.Infrastructure.Repositories;
//using Solvelt.Application.Common.Interfaces;
//using Solvelt.Infrastructure.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;


//var builder = WebApplication.CreateBuilder(args);

//// Controllers
//builder.Services.AddControllers();

//// Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// DbContext
//builder.Services.AddDbContext<SolveItDbContext>(options =>
//    options.UseNpgsql(
//        builder.Configuration.GetConnectionString("DefaultConnection")));

//// MediatR
//builder.Services.AddMediatR(cfg =>
//    cfg.RegisterServicesFromAssembly(typeof(RegisterOrganizerHandler).Assembly));

//// FluentValidation
//builder.Services.AddFluentValidationAutoValidation();
//builder.Services.AddValidatorsFromAssembly(typeof(RegisterOrganizerCommand).Assembly);

//// JwtTokenService
//builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


//// Repositories
//builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();
//builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


//var jwtSection = builder.Configuration.GetSection("Jwt");
//var secretKey = jwtSection["SecretKey"];

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,

//        ValidIssuer = jwtSection["Issuer"],
//        ValidAudience = jwtSection["Audience"],

//        IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(secretKey!))
//    };
//});


//var app = builder.Build();

//// Pipeline
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//// Global Exception Handling Middleware
//app.UseMiddleware<ExceptionMiddleware>();

//app.UseAuthentication();
//app.UseAuthorization();
//app.MapControllers();

//app.Run();



using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolveIt.Infrastructure.Persistence;
using Solvelt.Api.Middleware;
using Solvelt.Application.Common.Interfaces;
using Solvelt.Application.Interfaces;
using Solvelt.Application.Organizers.Commands.RegisterOrganizer;
using Solvelt.Infrastructure.Repositories;
using Solvelt.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger + JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter only the JWT token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// DbContext
builder.Services.AddDbContext<SolveItDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegisterOrganizerHandler).Assembly));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(RegisterOrganizerCommand).Assembly);

// Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Repositories
builder.Services.AddScoped<IOrganizerRepository, OrganizerRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// JWT Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey!))
    };
});

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Global Exception Handling
app.UseMiddleware<ExceptionMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

