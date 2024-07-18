using Api.Middleware;
using Data;
using Data.Repositories;
using Domain.Services;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Reflection;
using System.Text;
using AutoMapper;
using Api.Hubs;
using EntityFramework.DbContextScope.Interfaces;
using EntityFramework.DbContextScope;

string _corsPolicy = "EnableAll";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<JeopardyConfigurationOptions>(builder.Configuration);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("172.17.0.1"));
});

builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Jeopardy API",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Please provide authorization token to access restricted features.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
            new List<string>()
        }
    });

    c.AddSignalRSwaggerGen();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _corsPolicy,
                    builder =>
                    {
                        builder
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowAnyOrigin();
                    });
});

var connectionString = builder.Configuration.GetConnectionString("MySQL");
builder.Services.AddDbContext<JeopardyContext>(options =>
options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build());
});

// Add the following code to register DbContextScopeFactory
builder.Services.AddScoped<IDbContextScopeFactory>(provider =>
{
    var dbContextScopeFactory = new DbContextScopeFactory();
    return dbContextScopeFactory;
});

AddLifeCycles(builder);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(_corsPolicy);

app.UseRouting();

app.UseHttpsRedirection();
app.UseJwtMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<GameSessionHub>("/gameSessionHub");
    endpoints.MapSwagger();
});

app.MapControllers();

await ApplyMigrations(app);

app.Run();

static async Task ApplyMigrations(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    using var db = scope.ServiceProvider.GetService<JeopardyContext>();
    await db.Database.MigrateAsync();
}

static void AddLifeCycles(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    builder.Services.AddScoped<IProfessorRepository, ProfessorRepository>();
    builder.Services.AddScoped<IProfessorService, ProfessorService>();
    builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
    builder.Services.AddScoped<IQuestionService, QuestionService>();
    builder.Services.AddScoped<IThemeRepository, ThemeRepository>();
    builder.Services.AddScoped<IThemeService, ThemeService>();
    builder.Services.AddScoped<ITeamRepository, TeamRepository>();
    builder.Services.AddScoped<ITeamService, TeamService>();
    builder.Services.AddScoped<IMemberRepository, MemberRepository>();
    builder.Services.AddScoped<IQuestionOfPackageRepository, QuestionOfPackageRepository>();
    builder.Services.AddScoped<IQuestionOfPackageService, QuestionOfPackageService>();
    builder.Services.AddScoped<IPackageRepository, PackageRepository>();
    builder.Services.AddScoped<IPackageService, PackageService>();
    builder.Services.AddScoped<ISessionRepository, SessionRepository>();
    builder.Services.AddScoped<ISessionService, SessionService>();
    builder.Services.AddSignalR();
}
