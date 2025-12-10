using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// Токен и URL webhook читаются из конфигурации:
// - Для разработки: используйте User Secrets (dotnet user-secrets set "BotToken" "ваш_токен")
// - Для продакшена: используйте переменные окружения (BotToken, BotWebhookUrl)
var token = builder.Configuration["BotToken"]!;
var webhookUrl = builder.Configuration["BotWebhookUrl"]!;
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));

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

async Task<IResult> OnUpdate(TelegramBotClient bot, Update update)
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

        Console.WriteLine($"Received message '{message.Text}' in {message.Chat}");
        // let's echo back received text in the chat
        await bot.SendMessage(message.Chat, $"{message.From} said: {message.Text}");
        return Results.Ok();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing update: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        return Results.Ok();
    }
}