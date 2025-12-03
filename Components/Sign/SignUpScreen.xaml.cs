
using Ixjok.Services.Auth;

namespace Ixjok.Components.Sign;

public sealed partial class SignUpScreen : ContentPage
{
    public SignUpScreen(SignUpViewModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}
public sealed partial class SignUpViewModel(INavigationManager navigation, IWorkSpace work, IAuthentication authentication) : BaseViewModel(navigation)
{
    [RelayCommand]
    private async Task GotoBack() => await _navigation.BackAsync();
    [RelayCommand]
    private async Task SignUpAsync()
    {
        NetworkAccess accessType = Connectivity.Current.NetworkAccess;
        if (accessType != NetworkAccess.Internet)
        {
            await Toast.Make("Sin Internet").Show();
            return;
        }
        var result = await authentication.SignUpAsync(new(UserName, Password));
        if (result.IsSuccess)
        {
            _ = Toast.Make("Sección Iniciada").Show();
            await work.ChangeModeAsync(StorageMode.Cloud);
            await _navigation.GotoHomeAsync();
            return;
        }
        switch (result.Status)
        {
            case ResultStatus.BadRequest:
                await Toast.Make("Credenciales Invalidas").Show();
                break;
            case ResultStatus.Conflict:
                await Toast.Make("Usuario Existente").Show();
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
    public partial bool IsValid { get; set; } = false;
}