using CommunityToolkit.Mvvm.ComponentModel;
using Ixjok.Services.Navigation;

namespace Ixjok.Components;

/// <summary>
/// Clase base abstracta para todos los ViewModels de la aplicación.
/// Proporciona acceso al gestor de navegación y hereda de <see cref="ObservableObject"/> para soporte de vinculación MVVM.
/// </summary>
public abstract partial class BaseViewModel(INavigationManager navigation) : ObservableObject
{
    /// <summary>
    /// Gestor de navegación para la aplicación. Protected para uso en clases derivadas.
    /// </summary>
    private protected readonly INavigationManager _navigation = navigation;
}