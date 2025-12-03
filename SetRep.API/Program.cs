using Microsoft.EntityFrameworkCore;
using SetRep.Core.Repositories;
using SetRep.Infrastructure.Data;
using SetRep.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
