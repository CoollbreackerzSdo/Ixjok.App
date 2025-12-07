using Ixjok.Components;
using Ixjok.Services.Auth;

namespace Ixjok;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    protected override Window CreateWindow(IActivationState? activationState)
    {
        Handler.MauiContext!.Services.GetRequiredService<IBearerAuthenticationHandler>();
        Handler.MauiContext!.Services.GetRequiredService<IDynamicStorage>();
        return new(Handler.MauiContext!.Services.GetRequiredService<Navigation>());
    }
}