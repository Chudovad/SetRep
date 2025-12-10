using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SetRep.API.Services;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;
using SetRep.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем DbContext для PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрируем репозитории
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<IWorkoutExerciseRepository, WorkoutExerciseRepository>();
builder.Services.AddScoped<ISetRepository, SetRepository>();
builder.Services.AddScoped<IWeightHistoryRepository, WeightHistoryRepository>();
builder.Services.AddScoped<IWorkoutHistoryRepository, WorkoutHistoryRepository>();
builder.Services.AddScoped<IWorkoutHistorySetRepository, WorkoutHistorySetRepository>();

// Регистрируем сервис валидации Telegram WebApp
builder.Services.AddScoped<ITelegramWebAppValidationService, TelegramWebAppValidationService>();

// Настройка CORS для доступа из WebApp и MiniApp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7001", "http://localhost:5186", // WebApp
                "https://localhost:7001", "http://localhost:5096"  // MiniApp
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Применяем миграции базы данных автоматически при старте приложения
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Применяем все непримененные миграции
        // Если база данных не существует, она будет создана автоматически
        context.Database.Migrate();
        Console.WriteLine("Миграции базы данных успешно применены.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла ошибка при применении миграций базы данных.");
        // Не прерываем запуск приложения, чтобы можно было увидеть ошибку в логах
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

// Используем HTTPS редирект только если настроен HTTPS порт
var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORTS"];
if (!string.IsNullOrEmpty(httpsPort))
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowWebApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
