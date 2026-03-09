using Microsoft.AspNetCore.Components.Forms;

namespace SetRep.MiniApp.Client.Services;

/// <summary>
/// WASM-реализация IImageConversionService.
/// Magick.NET недоступен в браузере, поэтому конвертация HEIC не выполняется.
/// </summary>
public class NoOpImageConversionService : IImageConversionService
{
    public Task<Stream?> ConvertToJpegIfNeededAsync(IBrowserFile file, CancellationToken cancellationToken = default)
        => Task.FromResult<Stream?>(null);

    public bool IsHeicFormat(IBrowserFile file) => false;
}
