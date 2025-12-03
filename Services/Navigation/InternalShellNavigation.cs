
namespace Ixjok.Services.Navigation;

public sealed class InternalShellNavigation : INavigationManager
{
    public async Task BackAsync(bool animated = false, CancellationToken token = default)
        => await Shell.Current.GoToAsync("..", animated);
    public async Task GoToAsync(string route, bool animated = false, CancellationToken token = default)
         => await Shell.Current.GoToAsync(route, animated);
    public async Task GoToAsync(string route, IDictionary<string, object?> query, bool animated = false, CancellationToken token = default)
        => await Shell.Current.GoToAsync(route, animated, query);
    public async Task GotoHomeAsync(CancellationToken token = default)
        => await Shell.Current.GoToAsync("///Home", false);
}