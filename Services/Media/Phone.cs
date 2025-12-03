namespace Ixjok.Services.Media;

public sealed class Phone : IPictureProvider
{
    public async Task<Result<FileResult>> MakePicture(CancellationToken token = default)
    {
        if (await Permissions.CheckStatusAsync<Permissions.Camera>() == PermissionStatus.Denied && await Permissions.RequestAsync<Permissions.Camera>() == PermissionStatus.Disabled)
            return Result.UnAuthorized();
        if (!MediaPicker.IsCaptureSupported) return Result.NoSupport();
        var result = await MediaPicker.Default.CapturePhotoAsync();
        return result is null ? Result.Canceled() : result;
    }
}