
using Ixjok.Services.Auth;

namespace Ixjok.Components.Sign;

public sealed partial class SignUpScreen : ContentPage
{
    public SignUpScreen(SignUpViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        TVB.ValidationChange += UpdateValidation;
        TVE.ValidationChange += UpdateValidation;
        TVP.ValidationChange += UpdateValidation;
    }
    public void UpdateValidation(bool value) => ((SignUpViewModel)BindingContext).IsValid = value;
}
public sealed partial class SignUpViewModel(INavigationManager navigation, IAuthentication authentication) : BaseViewModel(navigation)
{
    [RelayCommand]
    private async Task GotoBack() => await _navigation.BackAsync();
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
    public SignUpRequest Request { get; init; } = new();
    [ObservableProperty]
    public partial bool IsValid { get; set; }
}