using Ixjok.Components;

namespace Ixjok;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }
    protected override Window CreateWindow(IActivationState? activationState) => new(Handler.MauiContext!.Services.GetRequiredService<Navigation>());
}