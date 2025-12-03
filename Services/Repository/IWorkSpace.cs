using Ixjok.Models.Note;

namespace Ixjok.Services.Repository;

public interface IWorkSpace : IDisposable
{
    public Action? Refresh { get; set; }
    Task ChangeModeAsync(StorageMode mode);
    IRepository<NoteModel> NoteRepository { get; }
    StorageMode Mode { get; }
}