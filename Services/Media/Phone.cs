namespace Ixjok.Services.Media;

/// <summary>
/// Implementación de <see cref="IPictureProvider"/> que captura fotos usando la cámara del dispositivo.
/// Solicita permisos de cámara si es necesario.
/// </summary>
public sealed class Phone : IPictureProvider
{
    /// <summary>
    /// Captura una foto de forma asíncrona desde la cámara del dispositivo.
    /// Verifica y solicita permisos de cámara si es necesario.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que contiene el archivo capturado o un código de error.</returns>
    public async Task<Result<FileResult>> MakePicture(CancellationToken token = default)
    {
        if (await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Denied && await Permissions.RequestAsync<Permissions.Camera>() == PermissionStatus.Disabled)
            return Result.UnAuthorized();
        if (!MediaPicker.IsCaptureSupported) return Result.NoSupport();
        var result = await MediaPicker.Default.CapturePhotoAsync();
        return result is null ? Result.Canceled() : result;
    }
}