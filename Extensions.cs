using System.Globalization;
using System.Threading.Channels;
using Ixjok.Components;
using Ixjok.Components.Principal;
using Ixjok.Components.Sign;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;
using Microsoft.Extensions.Hosting;

namespace Ixjok;

/// <summary>
/// Métodos de extensión para configurar servicios de inyección de dependencias en la aplicación.
/// Registra autenticación, pantallas, ViewModels, navegación, almacenamientos y conversiones de tipos.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Aplica la configuración de la aplicación creando el directorio de base de datos si no existe.
    /// </summary>
    /// <param name="builder">El constructor de aplicación hospedada.</param>
    /// <returns>El constructor de aplicación para encadenamiento de métodos.</returns>
    public static IHostApplicationBuilder ApplyConfiguration(this IHostApplicationBuilder builder)
    {
        var directoryPath = ConfigHelper.DatabaseDirectory;
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        // builder.Services.AddSingleton<ConfigHelper>();
        return builder;
    }

    /// <summary>
    /// Registra los servicios de autenticación en el contenedor de DI.
    /// Configura el gestor de autenticación con tokens Bearer JWT.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La colección de servicios para encadenamiento de métodos.</returns>
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
        => services.AddSingleton<JwtHostedAuthenticationHandler>()
            .AddSingleton<IBearerAuthenticationHandler>(op => op.GetRequiredService<JwtHostedAuthenticationHandler>())
            .AddSingleton<IAuthentication>(op => op.GetRequiredService<IBearerAuthenticationHandler>());

    /// <summary>
    /// Registra las pantallas de la aplicación y sus rutas de navegación.
    /// Registra como singleton: Navigation, NoteScreen.
    /// Registra como transient: NoteEditorScreen, SignInScreen, SignUpScreen.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La colección de servicios para encadenamiento de métodos.</returns>
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

    /// <summary>
    /// Registra los ViewModels en el contenedor de DI.
    /// Registra como singleton: NoteViewModel.
    /// Registra como transient: SignInViewModel, SignUpViewModel, EditorNoteViewModel.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La colección de servicios para encadenamiento de métodos.</returns>
    public static IServiceCollection AddViewModels(this IServiceCollection services)
        => services.AddSingleton<NoteViewModel>()
                   .AddTransient<SignInViewModel>()
                   .AddTransient<SignUpViewModel>()
                   .AddTransient<EditorNoteViewModel>();

    /// <summary>
    /// Registra el gestor de navegación en el contenedor de DI.
    /// Utiliza <see cref="InternalShellNavigation"/> como implementación de <see cref="INavigationManager"/>.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La colección de servicios para encadenamiento de métodos.</returns>
    public static IServiceCollection AddNavigation(this IServiceCollection services)
        => services.AddSingleton<INavigationManager, InternalShellNavigation>();

    /// <summary>
    /// Registra los servicios de medios en el contenedor de DI.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La colección de servicios para encadenamiento de métodos.</returns>
    public static IServiceCollection AddMediaServices(this IServiceCollection services)
        => services.AddSingleton<IPictureProvider, Phone>();

    /// <summary>
    /// Registra los almacenamientos (repositorios) en el contenedor de DI.
    /// Configura DynamicStorage para notas locales y HostedNoteStorage para notas remotas.
    /// </summary>
    /// <param name="services">La colección de servicios.</param>
    /// <returns>La colección de servicios para encadenamiento de métodos.</returns>
    public static IServiceCollection AddStorages(this IServiceCollection services)
        => services.AddSingleton<IDynamicStorage, DynamicStorage>()
        .AddSingleton<IHostedNoteRepository, HostedNoteStorage>();

    /// <summary>
    /// Extensión para convertir un <see cref="NoteModel"/> a <see cref="NoteSqlDecorator"/> para persistencia.
    /// </summary>
    /// <param name="model">La nota a convertir.</param>
    /// <returns>Un adaptador SQL que representa la nota.</returns>
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

    /// <summary>
    /// Extensión para convertir un <see cref="NoteSqlDecorator"/> a <see cref="NoteModel"/> desde persistencia.
    /// </summary>
    /// <param name="model">El adaptador SQL a convertir.</param>
    /// <returns>Un modelo de nota con datos deserealizados.</returns>
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