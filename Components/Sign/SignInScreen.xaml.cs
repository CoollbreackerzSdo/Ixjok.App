
using Ixjok.Services.Auth;

namespace Ixjok.Components.Sign;

public sealed partial class SignInScreen : ContentPage
{
    public SignInScreen(SignInViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
        TVB.ValidationChange += UpdateValidation;
        TVP.ValidationChange += UpdateValidation;
    }
    public void UpdateValidation(bool value) => ((SignInViewModel)BindingContext).IsValid = value;
}
public sealed partial class SignInViewModel(INavigationManager navigation, IAuthentication authentication) : BaseViewModel(navigation)
{
    [RelayCommand]
    private async Task GotoBack() => await _navigation.BackAsync();
    [RelayCommand]
    private async Task GotoSignUp() => await _navigation.GoToAsync(nameof(SignUpScreen));
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
    public SignInRequest Request { get; init; } = new();
    [ObservableProperty]
    public partial bool IsValid { get; set; }
}