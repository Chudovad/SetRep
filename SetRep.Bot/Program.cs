using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// Токен и URL webhook читаются из конфигурации:
// - Для разработки: используйте User Secrets (dotnet user-secrets set "BotToken" "ваш_токен")
// - Для продакшена: используйте переменные окружения (BotToken, BotWebhookUrl)
var token = builder.Configuration["BotToken"]!;
var webhookUrl = builder.Configuration["BotWebhookUrl"]!;
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5181";

builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));
builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Используем HTTPS редирект только если настроен HTTPS порт
var httpsPort = app.Configuration["ASPNETCORE_HTTPS_PORTS"];
if (!string.IsNullOrEmpty(httpsPort))
{
    app.UseHttpsRedirection();
}

app.MapGet("/bot/setWebhook", async (TelegramBotClient bot) => { await bot.SetWebhook(webhookUrl); return $"Webhook set to {webhookUrl}"; });
app.MapPost("/bot", OnUpdate).Accepts<Update>("application/json");

app.Run();

async Task<IResult> OnUpdate(TelegramBotClient bot, IHttpClientFactory httpClientFactory, Update update)
{
    try
    {
        if (update is null)
        {
            Console.WriteLine("Received null update");
            return Results.Ok();
        }

        var message = update.Message ?? update.EditedMessage;

        if (message is null)
        {
            Console.WriteLine("No message found in update");
            return Results.Ok();
        }

        if (message.Text is null)
        {
            Console.WriteLine("Message text is null");
            return Results.Ok();
        }

        Console.WriteLine($"Received message '{message.Text}' from user {message.From?.Id}");

        if (message.Text.StartsWith("/start", StringComparison.OrdinalIgnoreCase))
        {
            await HandleStartCommand(bot, httpClientFactory, message);
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing update: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return Results.Ok();
    }
}

async Task HandleStartCommand(TelegramBotClient bot, IHttpClientFactory httpClientFactory, Message message)
{
    if (message.From is null)
    {
        return;
    }

    var httpClient = httpClientFactory.CreateClient("api");
    var telegramId = message.From.Id;

    try
    {
        var checkResponse = await httpClient.GetAsync($"/api/users/telegram/{telegramId}");
        
        if (!checkResponse.IsSuccessStatusCode)
        {
            var user = new
            {
                Id = Guid.NewGuid(),
                TelegramId = telegramId,
                Name = message.From.FirstName ?? string.Empty,
                Username = message.From.Username ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var createResponse = await httpClient.PostAsync("/api/users", content);

            if (createResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"User {telegramId} created successfully");
            }
            else
            {
                var errorContent = await createResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to create user {telegramId}: {errorContent}");
            }
        }
        else
        {
            Console.WriteLine($"User {telegramId} already exists");
        }

        var welcomeMessage = "👋 Добро пожаловать в SetRep!\n\n" +
                            "Этот бот поможет вам отслеживать ваши тренировки и прогресс.\n\n" +
                            "📱 Для работы с приложением используйте веб-приложение через кнопку меню бота.\n\n" +
                            "💪 Начните создавать тренировки и отслеживать свои результаты!";

        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: welcomeMessage
        );
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error handling /start command: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: "❌ Произошла ошибка при регистрации. Пожалуйста, попробуйте позже."
        );
    }
}