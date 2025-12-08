using System.Collections.ObjectModel;

namespace Ixjok.Services.Repository;

/// <summary>
/// Define la interfaz principal para un repositorio genérico con capacidades CRUD.
/// Agrega, actualiza, elimina y obtiene modelos de tipo T.
/// </summary>
/// <typeparam name="T">El tipo de modelo gestionado por el repositorio.</typeparam>
public interface IRepository<T> : IUpdateFeature<T>, IGetFeature<T>, IAddFeature<T>, IDeleteFeature<T>, IDisposable { }

/// <summary>
/// Define las capacidades de actualización para un repositorio.
/// </summary>
/// <typeparam name="T">El tipo de modelo a actualizar.</typeparam>
public interface IUpdateFeature<T>
{
    /// <summary>
    /// Actualiza un modelo de forma asíncrona.
    /// </summary>
    /// <param name="model">El modelo a actualizar.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task UpdateAsync(T model, CancellationToken token = default);
}

/// <summary>
/// Define las capacidades de eliminación para un repositorio.
/// </summary>
/// <typeparam name="T">El tipo de modelo a eliminar.</typeparam>
public interface IDeleteFeature<T>
{
    /// <summary>
    /// Elimina un modelo de forma asíncrona.
    /// </summary>
    /// <param name="model">El modelo a eliminar.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task RemoveAsync(T model, CancellationToken token = default);
}

/// <summary>
/// Define las capacidades de adición para un repositorio.
/// </summary>
/// <typeparam name="T">El tipo de modelo a agregar.</typeparam>
public interface IAddFeature<T>
{
    /// <summary>
    /// Agrega un nuevo modelo de forma asíncrona.
    /// </summary>
    /// <param name="model">El modelo a agregar.</param>
    /// <param name="token">Token de cancelación para la operación asíncrona.</param>
    /// <returns>Una tarea que representa la operación asíncrona.</returns>
    Task AddAsync(T model, CancellationToken token = default);
}

/// <summary>
/// Define las capacidades de lectura para un repositorio.
/// </summary>
/// <typeparam name="T">El tipo de modelo a obtener.</typeparam>
public interface IGetFeature<T>
{
    /// <summary>
    /// Obtiene una colección observable de todos los modelos.
    /// </summary>
    /// <returns>Una colección observable de modelos de tipo T.</returns>
    ObservableCollection<T> GetObservableAll();
}