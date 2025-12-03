using Ixjok.Components;
using Ixjok.Components.Principal;
using Ixjok.Components.Sign;
using Ixjok.Services.Auth;
using Microsoft.Extensions.Hosting;

namespace Ixjok;

public static partial class Extensions
{
    public static IHostApplicationBuilder ApplyConfiguration(this IHostApplicationBuilder builder)
    {
        var directoryPath = ConfigHelper.DatabaseDirectory;
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        return builder;
    }
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
        => services.AddSingleton<DefaultAuthentication>()
            .AddSingleton<IBearerAuthentication>(op => op.GetRequiredService<DefaultAuthentication>())
            .AddSingleton<IAuthentication>(op => op.GetRequiredService<IBearerAuthentication>());
    public static IServiceCollection AddScreens(this IServiceCollection services)
    {
        Routing.RegisterRoute(nameof(SignInScreen), typeof(SignInScreen));
        Routing.RegisterRoute(nameof(SignUpScreen), typeof(SignUpScreen));
        Routing.RegisterRoute(nameof(NoteEditorScreen), typeof(NoteEditorScreen));

        return services.AddSingleton<Navigation>()
                       .AddSingleton<NoteScreen>()
                       .AddTransient<NoteEditorScreen>()
                       .AddTransient<SignInScreen>()
                       .AddTransient<SignUpScreen>();
    }
    public static IServiceCollection AddViewModels(this IServiceCollection services)
        => services.AddSingleton<NoteViewModel>()
                   .AddTransient<SignInViewModel>()
                   .AddTransient<SignUpViewModel>()
                   .AddTransient<EditorNoteViewModel>();
    public static IServiceCollection AddNavigation(this IServiceCollection services)
        => services.AddSingleton<INavigationManager, InternalShellNavigation>();
    public static IServiceCollection AddMediaServices(this IServiceCollection services)
        => services.AddSingleton<IPictureProvider, Phone>();
    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddSingleton<IWorkSpace, WorkSpace>();
}