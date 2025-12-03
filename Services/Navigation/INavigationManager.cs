namespace Ixjok.Services.Navigation;

public interface INavigationManager
{
    public Task GotoHomeAsync(CancellationToken token = default);
    public Task GoToAsync(string route, bool animated = false, CancellationToken token = default);
    public Task GoToAsync(string route, IDictionary<string, Object?> query, bool animated = false, CancellationToken token = default);
    public Task BackAsync(bool animated = false, CancellationToken token = default);
}