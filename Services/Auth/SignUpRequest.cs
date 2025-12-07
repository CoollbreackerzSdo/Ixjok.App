using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ixjok.Services.Auth;

public sealed partial class SignUpRequest : INotifyPropertyChanged
{
    public string? Email
    {
        get; set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string? UserName
    {
        get; set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string? Password
    {
        get; set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    public event PropertyChangedEventHandler? PropertyChanged;
}