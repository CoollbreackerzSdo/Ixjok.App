using Ixjok.Models.Note;

namespace Ixjok.Services.Repository;

public interface IHostedNoteRepository : IDisposable
{
    Task<Result> UpdateAsync(NoteModel model, CancellationToken token = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken token = default);
    Task<Result> AddAsync(NoteModel model, CancellationToken token = default);
    IAsyncEnumerable<NoteModel> GetAllAsync(CancellationToken cancellationToken = default);
}