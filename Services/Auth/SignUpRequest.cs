using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ixjok.Services.Auth;

/// <summary>
/// Solicitud de registro de un nuevo usuario.
/// Implementa <see cref="INotifyPropertyChanged"/> para notificar cambios en la UI.
/// </summary>
public sealed partial class SignUpRequest : INotifyPropertyChanged
{
    /// <summary>
    /// Obtiene o establece el correo electrónico del usuario.
    /// </summary>
    public string? Email
    {
        get; set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    /// <summary>
    /// Obtiene o establece el nombre de usuario.
    /// </summary>
    public string? UserName
    {
        get; set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    /// <summary>
    /// Obtiene o establece la contraseña.
    /// </summary>
    public string? Password
    {
        get; set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    /// <summary>
    /// Dispara el evento <see cref="PropertyChanged"/> cuando una propiedad cambia.
    /// </summary>
    /// <param name="name">Nombre de la propiedad que cambió.</param>
    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    /// <summary>
    /// Evento que se dispara cuando una propiedad cambia.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
}