using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using WebApplication1.Data;
using WebApplication1.Repository.Impl;
using WebApplication1.Repository.Interface;
using WebApplication1.Service.Impl;
using WebApplication1.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

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
// Inside Program.cs
builder.Services.AddScoped<ISmsService, SmsSender>();
// Add this to register HttpClient in the DI container
builder.Services.AddHttpClient();





builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        
        options.JsonSerializerOptions.WriteIndented = true;
    });

try
{
    //string connStr = builder.Configuration.GetConnectionString("MySqlConnection");

    //using var connection = new MySqlConnection(connStr);

    //connection.Open();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
