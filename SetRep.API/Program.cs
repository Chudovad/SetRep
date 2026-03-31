using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SetRep.API.Options;
using SetRep.API.Services;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;
using SetRep.Infrastructure.Repositories;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.Configure<FileUploadOptions>(
    builder.Configuration.GetSection(FileUploadOptions.SectionName));
builder.Services.Configure<ReadyWorkoutPlanOptions>(
    builder.Configuration.GetSection(ReadyWorkoutPlanOptions.SectionName));

var fileUploadOptions = builder.Configuration.GetSection(FileUploadOptions.SectionName)
    .Get<FileUploadOptions>() ?? new FileUploadOptions();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = fileUploadOptions.MaxFileSizeBytes;
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Добавляем Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SetRep API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter '{token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Добавляем DbContext для PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрируем репозитории
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>();
builder.Services.AddScoped<IWorkoutExerciseRepository, WorkoutExerciseRepository>();
builder.Services.AddScoped<ISetRepository, SetRepository>();
builder.Services.AddScoped<IMeasurementsHistoryRepository, MeasurementsHistoryRepository>();
builder.Services.AddScoped<IWorkoutHistoryRepository, WorkoutHistoryRepository>();
builder.Services.AddScoped<IWorkoutHistorySetRepository, WorkoutHistorySetRepository>();
builder.Services.AddScoped<IReadyWorkoutPlanRepository, ReadyWorkoutPlanRepository>();

// Регистрируем сервис валидации Telegram WebApp
builder.Services.AddScoped<ITelegramWebAppValidationService, TelegramWebAppValidationService>();

// Регистрируем сервис JWT токенов
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Регистрируем сервис Yandex Object Storage
builder.Services.AddScoped<IYandexObjectStorageService, YandexObjectStorageService>();

// Регистрируем фоновый сервис для автоматического завершения тренировок
builder.Services.AddHostedService<WorkoutAutoCompletionService>();

// Настройка JWT аутентификации
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SetRep.API";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SetRep.Client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Настройка CORS для доступа из WebApp и MiniApp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
              {
                  var allowedOrigins = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                  {
                      "https://localhost:7001",
                      "http://localhost:5186",
                      "http://localhost:5096"
                  };
                  if (allowedOrigins.Contains(origin))
                      return true;
                  return Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                      && uri.Host.EndsWith(".devtunnels.ms", StringComparison.OrdinalIgnoreCase);
              })
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        Console.WriteLine("Миграции базы данных успешно применены.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла ошибка при применении миграций базы данных.");
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
