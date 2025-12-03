using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Ixjok.Components.Controls;

namespace Ixjok;

[Application(UsesCleartextTraffic = true)]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
        {
            if (view is FillEntry)
            {
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                handler.PlatformView.TextCursorDrawable?.SetTint(Android.Graphics.Color.White);
            }
        });

        Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) =>
        {
            if (view is FillEditor)
            {
                handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                handler.PlatformView.TextCursorDrawable?.SetTint(Android.Graphics.Color.White);
            }
        });
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}