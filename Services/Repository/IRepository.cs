using System.Collections.ObjectModel;

namespace Ixjok.Services.Repository;

public interface IRepository<TModel> : IDisposable
{
    ObservableCollection<TModel> GetObservableAll();
    IEnumerable<TModel> GetAll();
    Task<Result> AddAsync(TModel model, CancellationToken token = default);
    Task<Result> RemoveAsync(TModel model, CancellationToken token = default);
    Task<Result> UpdateAsync(TModel model, CancellationToken token = default);
}