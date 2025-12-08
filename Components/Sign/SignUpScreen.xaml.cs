
using Ixjok.Services.Auth;

namespace Ixjok.Components.Sign;

/// <summary>
/// Pantalla de registro para creación de nuevas cuentas de usuario.
/// Se vincula con <see cref="SignUpViewModel"/> y gestiona la validación de campos de registro.
/// </summary>
public sealed partial class SignUpScreen : ContentPage
{
    /// <summary>
    /// Inicializa una nueva instancia de <see cref="SignUpScreen"/>.
    /// Registra validadores de campos para nombre de usuario, email y contraseña.
    /// </summary>
    /// <param name="model">El ViewModel de registro inyectado por DI.</param>
    public SignUpScreen(SignUpViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        TVB.ValidationChange += UpdateValidation;
        TVE.ValidationChange += UpdateValidation;
        TVP.ValidationChange += UpdateValidation;
    }

    /// <summary>
    /// Actualiza el estado de validación general cuando cambian los campos.
    /// </summary>
    /// <param name="value">Indica si todos los campos son válidos.</param>
    public void UpdateValidation(bool value) => ((SignUpViewModel)BindingContext).IsValid = value;
}

/// <summary>
/// ViewModel para la pantalla de registro de usuario.
/// Gestiona datos de registro, validación y creación de cuenta.
/// </summary>
public sealed partial class SignUpViewModel(INavigationManager navigation, IAuthentication authentication) : BaseViewModel(navigation)
{
    /// <summary>
    /// Comando para regresar a la pantalla anterior.
    /// </summary>
    [RelayCommand]
    private async Task GotoBack() => await _navigation.BackAsync();

    /// <summary>
    /// Comando para registrar un nuevo usuario.
    /// Valida los campos y envía los datos de registro al servidor.
    /// </summary>
    [RelayCommand]
    private async Task SignUpAsync()
    {
        if (!IsValid)
        {
            _ = Toast.Make("Credenciales Invalidas").Show();
            return;
        }
        var result = await authentication.SignUpAsync(Request);
        if (result.IsSuccess)
        {
            _ = Toast.Make("Registro Exitoso", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
            await _navigation.GotoHomeAsync();
            return;
        }
        _ = result.Status switch
        {
            ResultStatus.Conflict => Toast.Make("Usuario Registrado").Show(),
            ResultStatus.BadRequest => Toast.Make("Credenciales Invalidas").Show(),
            _ => Toast.Make("Error Externo").Show(),
        };
    }

    /// <summary>
    /// Obtiene la solicitud de registro con nombre de usuario, email y contraseña.
    /// </summary>
    public SignUpRequest Request { get; init; } = new();

    /// <summary>
    /// Obtiene o establece si todos los campos son válidos.
    /// </summary>
    [ObservableProperty]
    public partial bool IsValid { get; set; }
}