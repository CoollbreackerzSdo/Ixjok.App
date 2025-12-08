
using Ixjok.Services.Auth;

namespace Ixjok.Components.Sign;

/// <summary>
/// Pantalla de inicio de sesión para autenticación de usuarios.
/// Se vincula con <see cref="SignInViewModel"/> y gestiona la validación de campos.
/// </summary>
public sealed partial class SignInScreen : ContentPage
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="SignInScreen"/>.
    /// Registra validadores de campos para nombre de usuario y contraseña.
    /// </summary>
    /// <param name="model">El ViewModel de inicio de sesión inyectado por DI.</param>
    public SignInScreen(SignInViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        TVB.ValidationChange += UpdateValidation;
        TVP.ValidationChange += UpdateValidation;
    }

    /// <summary>
    /// Actualiza el estado de validación general cuando cambian los campos.
    /// </summary>
    /// <param name="value">Indica si todos los campos son válidos.</param>
    public void UpdateValidation(bool value) => ((SignInViewModel)BindingContext).IsValid = value;
}

/// <summary>
/// ViewModel para la pantalla de inicio de sesión.
/// Gestiona credenciales, validación y autenticación del usuario.
/// </summary>
public sealed partial class SignInViewModel(INavigationManager navigation, IAuthentication authentication) : BaseViewModel(navigation)
{
    /// <summary>
    /// Comando para regresar a la pantalla anterior.
    /// </summary>
    [RelayCommand]
    private async Task GotoBack() => await _navigation.BackAsync();

    /// <summary>
    /// Comando para navegar a la pantalla de registro.
    /// </summary>
    [RelayCommand]
    private async Task GotoSignUp() => await _navigation.GoToAsync(nameof(SignUpScreen));

    /// <summary>
    /// Comando para iniciar sesión con las credenciales proporcionadas.
    /// Valida los campos y autentica al usuario contra el servidor.
    /// </summary>
    [RelayCommand]
    private async Task SignInAsync()
    {
        if (!IsValid)
        {
            _ = Toast.Make("Credenciales Invalidas").Show();
            return;
        }
        var result = await authentication.SignInAsync(Request);
        if (result.IsSuccess)
        {
            _ = Toast.Make("Session Exitosa", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
            await _navigation.GotoHomeAsync();
            return;
        }
        _ = result.Status switch
        {
            ResultStatus.NotFound => Toast.Make("No Registrado").Show(),
            ResultStatus.BadRequest => Toast.Make("Credenciales Invalidas").Show(),
            _ => Toast.Make("Error Externo").Show(),
        };
    }

    /// <summary>
    /// Obtiene la solicitud de inicio de sesión con nombre de usuario y contraseña.
    /// </summary>
    public SignInRequest Request { get; init; } = new();

    /// <summary>
    /// Obtiene o establece si todos los campos son válidos.
    /// </summary>
    [ObservableProperty]
    public partial bool IsValid { get; set; }
}