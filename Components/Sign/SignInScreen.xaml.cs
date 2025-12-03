
using Ixjok.Services.Auth;

namespace Ixjok.Components.Sign;

public sealed partial class SignInScreen : ContentPage
{
    public SignInScreen(SignInViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
public sealed partial class SignInViewModel(INavigationManager navigation, IWorkSpace work, IAuthentication authentication) : BaseViewModel(navigation)
{
    [RelayCommand]
    private async Task GotoBack() => await _navigation.BackAsync();
    [RelayCommand]
    private async Task GotoSignUp() => await _navigation.GoToAsync(nameof(SignUpScreen));
    [RelayCommand]
    private async Task SignInAsync()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;
        if (accessType != NetworkAccess.Internet)
        {
            await Toast.Make("Sin Internet").Show();
            return;
        }
        var result = await authentication.SignInAsync(new(UserName, Password));
        if (result.IsSuccess)
        {
            _ = Toast.Make("Sección Iniciada", CommunityToolkit.Maui.Core.ToastDuration.Long).Show();
            await work.ChangeModeAsync(StorageMode.Cloud);
            await _navigation.BackAsync();
            return;
        }
        switch (result.Status)
        {
            case ResultStatus.BadRequest:
                await Toast.Make("Contraseña Invalida").Show();
                break;
            case ResultStatus.NotFound:
                await Toast.Make("No Registrado").Show();
                break;
            default:
                await Toast.Make("ErrorX010101").Show();
                break;
        }
    }
    [ObservableProperty]
    public partial string UserName { get; set; }
    [ObservableProperty]
    public partial string Password { get; set; }
    [ObservableProperty]
    public partial bool IsValid { get; set; }
}