using Ixjok.Models.Note;

namespace Ixjok.Services.Repository;

public interface IDynamicStorage : IRepository<NoteModel>, IDisposable
{
    Task<Result> ConnectToHostAsync(CancellationToken token);
}
