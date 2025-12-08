using Ixjok.Components;
using Ixjok.Services.Auth;

namespace Ixjok;

/// <summary>
/// Clase principal de la aplicación MAUI.
/// Inicializa los servicios de autenticación y almacenamiento, y crea la ventana principal.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="App"/>.
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Crea la ventana principal de la aplicación.
    /// Inicializa los servicios clave de autenticación y almacenamiento dinámico.
    /// </summary>
    /// <param name="activationState">Estado de activación de la aplicación.</param>
    /// <returns>La ventana principal con navegación configurada.</returns>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        Handler.MauiContext!.Services.GetRequiredService<IBearerAuthenticationHandler>();
        Handler.MauiContext!.Services.GetRequiredService<IDynamicStorage>();
        return new(Handler.MauiContext!.Services.GetRequiredService<Navigation>());
    }
}