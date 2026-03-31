namespace SetRep.MiniApp.Middleware;

/// <summary>
/// Middleware для проксирования запросов /api/* от WASM-клиента к бэкенд API.
/// Браузер не может обращаться к внутренним адресам Docker-сети (http://api:8080),
/// поэтому WASM-клиент шлёт запросы на свой origin, а этот middleware перенаправляет их к API.
/// </summary>
public class ApiProxyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiProxyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IHttpClientFactory httpClientFactory)
    {
        if (!context.Request.Path.StartsWithSegments("/api", out var remaining))
        {
            await _next(context);
            return;
        }

        var client = httpClientFactory.CreateClient("ApiClient");
        var targetPath = $"/api{remaining}{context.Request.QueryString}";

        using var requestMessage = new HttpRequestMessage(new HttpMethod(context.Request.Method), targetPath);

        // Копируем заголовки запроса (кроме Host)
        foreach (var header in context.Request.Headers)
        {
            if (string.Equals(header.Key, "Host", StringComparison.OrdinalIgnoreCase))
                continue;

            requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        // Копируем тело запроса для методов с телом
        if (context.Request.ContentLength > 0 || context.Request.ContentType != null)
        {
            requestMessage.Content = new StreamContent(context.Request.Body);

            if (context.Request.ContentType != null)
            {
                requestMessage.Content.Headers.ContentType =
                    System.Net.Http.Headers.MediaTypeHeaderValue.Parse(context.Request.ContentType);
            }

            if (context.Request.ContentLength.HasValue)
            {
                requestMessage.Content.Headers.ContentLength = context.Request.ContentLength;
            }
        }

        using var response = await client.SendAsync(
            requestMessage,
            HttpCompletionOption.ResponseHeadersRead,
            context.RequestAborted);

        // Копируем статус ответа
        context.Response.StatusCode = (int)response.StatusCode;

        // Копируем заголовки ответа
        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        // Удаляем transfer-encoding чтобы избежать конфликтов
        context.Response.Headers.Remove("transfer-encoding");

        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
}
