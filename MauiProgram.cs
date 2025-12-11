using CommunityToolkit.Maui.Markup;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Ixjok;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Lato-Bold.ttf", "LB");
                fonts.AddFont("Lato-Light.ttf", "LL");
                fonts.AddFont("Lato-Regular.ttf", "LR");
                fonts.AddFont("StackSansNotch-Bold.ttf", "SB");
                fonts.AddFont("StackSansNotch-SemiBold.ttf", "SS");
            }).UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMarkup()
            .ApplyConfiguration()
            .Services
            .AddScreens()
            .AddAuthentication()
            .AddViewModels()
            .AddNavigation()
            .AddStorages()
            .AddMediaServices()
            .AddSingleton(op => new HttpClient { BaseAddress = new("https://ixjok.runasp.net/") });
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}