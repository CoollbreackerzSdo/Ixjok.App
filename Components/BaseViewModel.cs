using CommunityToolkit.Mvvm.ComponentModel;
using Ixjok.Services.Navigation;

namespace Ixjok.Components;

public abstract partial class BaseViewModel(INavigationManager navigation) : ObservableObject
{
    private protected readonly INavigationManager _navigation = navigation;
}