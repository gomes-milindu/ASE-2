using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Threading.RateLimiting;
using WebApplication1.Data;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);


// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("LoginPolicy", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromSeconds(10),
                PermitLimit = 3,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailService, EmailSender>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddScoped<ISmsService, SmsSender>();
builder.Services.AddTransient<IForgotPasswordRepository, ForgotPasswordRepository>();
builder.Services.AddHttpClient();

// Required to extract the IP Address from incoming HTTP requests
builder.Services.AddHttpContextAccessor();

// Register the new Audit Service
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

try
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(
                builder.Configuration.GetConnectionString("DefaultConnection")
            )
        )
    );

    Console.WriteLine("MySQL Connected Successfully");
}
catch (Exception ex)
{
    Console.WriteLine("MySQL Connection Failed");
    Console.WriteLine(ex.Message);
}

var app = builder.Build();

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseCors("AllowAll");
app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();