namespace Ixjok.Services.Media;

public interface IPictureProvider
{
    Task<Result<FileResult>> MakePicture(CancellationToken token = default);
}