using System.Collections.ObjectModel;

namespace Ixjok.Services.Repository;

public interface IRepository<T> : IUpdateFeature<T>, IGetFeature<T>, IAddFeature<T>, IDeleteFeature<T>, IDisposable { }
public interface IUpdateFeature<T>
{
    Task UpdateAsync(T model, CancellationToken token = default);
}
public interface IDeleteFeature<T>
{
    Task RemoveAsync(T model, CancellationToken token = default);
}
public interface IAddFeature<T>
{
    Task AddAsync(T model, CancellationToken token = default);
}
public interface IGetFeature<T>
{
    ObservableCollection<T> GetObservableAll();
}