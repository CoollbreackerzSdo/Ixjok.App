using Microsoft.Extensions.Logging;

namespace Ixjok;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Lato-Bold.ttf", "LB");
                fonts.AddFont("Lato-Light.ttf", "LL");
                fonts.AddFont("Lato-Regular.ttf", "LR");
                fonts.AddFont("StackSansNotch-Bold.ttf", "SB");
                fonts.AddFont("StackSansNotch-SemiBold.ttf", "SS");
            }).UseMauiCommunityToolkit().ConfigureMauiHandlers(op =>
            {
#if ANDROID
                op.AddHandler<Shell,ShellTrackerRenderer>();
#endif
            })
            .ApplyConfiguration()
            .Services
            .AddScreens()
            .AddAuthentication()
            .AddViewModels()
            .AddNavigation()
            .AddRepositories()
            .AddMediaServices()
            .AddSingleton(op => new HttpClient { BaseAddress = new("https://ixjok.runasp.net/") });
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}