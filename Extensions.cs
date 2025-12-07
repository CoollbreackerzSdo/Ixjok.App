using System.Globalization;
using System.Threading.Channels;
using Ixjok.Components;
using Ixjok.Components.Principal;
using Ixjok.Components.Sign;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;
using Microsoft.Extensions.Hosting;

namespace Ixjok;

public static partial class Extensions
{
    public static IHostApplicationBuilder ApplyConfiguration(this IHostApplicationBuilder builder)
    {
        var directoryPath = ConfigHelper.DatabaseDirectory;
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        // builder.Services.AddSingleton<ConfigHelper>();
        return builder;
    }
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
        => services.AddSingleton<JwtHostedAuthenticationHandler>()
            .AddSingleton<IBearerAuthenticationHandler>(op => op.GetRequiredService<JwtHostedAuthenticationHandler>())
            .AddSingleton<IAuthentication>(op => op.GetRequiredService<IBearerAuthenticationHandler>());
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
    public static IServiceCollection AddStorages(this IServiceCollection services)
        => services.AddSingleton<IDynamicStorage, DynamicStorage>()
        .AddSingleton<IHostedNoteRepository, HostedNoteStorage>();
    extension(NoteModel model)
    {
        public NoteSqlDecorator ToDecorator()
            => new()
            {
                Key = model.Id.ToString()!,
                Title = model.Title,
                Content = model.Content,
                Registration = model.Registration.ToString()
            };
    }
    extension(NoteSqlDecorator model)
    {
        public NoteModel ToModel()
            => new()
            {
                Id = Guid.Parse(model.Key!),
                Title = model.Title!,
                Content = model.Content!,
                Registration = DateTimeOffset.Parse(model.Registration!, CultureInfo.CurrentCulture)
            };
    }
}