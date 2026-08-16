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
    
    
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10); 
        opt.PermitLimit = 3;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});




// Add services to the container.

builder.Services.AddControllers();
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
// Inside Program.cs
builder.Services.AddScoped<ISmsService, SmsSender>();
// Add this to register HttpClient in the DI container
builder.Services.AddHttpClient();


// Required to extract the IP Address from incoming HTTP requests
builder.Services.AddHttpContextAccessor();

// Register the new Audit Service
builder.Services.AddScoped<IAuditService, AuditService>();





builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        
        options.JsonSerializerOptions.WriteIndented = true;
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


// Rate Limiter
app.UseRouting();
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
