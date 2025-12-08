namespace Ixjok.Services.Media;

/// <summary>
/// Define un proveedor de servicios para captura de fotos desde la cámara del dispositivo.
/// </summary>
public interface IPictureProvider
{
    /// <summary>
    /// Captura una foto de forma asíncrona.
    /// </summary>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Un resultado que contiene el archivo de foto capturado o un error.</returns>
    Task<Result<FileResult>> MakePicture(CancellationToken token = default);
}