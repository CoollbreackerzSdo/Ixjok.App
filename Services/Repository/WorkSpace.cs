using Ixjok.Models.Note;
using Ixjok.Services.Auth;

namespace Ixjok.Services.Repository;

public sealed partial class WorkSpace : IWorkSpace
{
    public WorkSpace(HttpClient client, IBearerAuthentication authentication)
    {
        _client = client;
        _authentication = authentication;
        if (_authentication.RequestAuthenticationStateAsync().GetAwaiter().GetResult())
        {
            NoteRepository = new CloudNoteRepository(_client, _authentication);
            Mode = StorageMode.Cloud;
        }
    }
    public async Task ChangeModeAsync(StorageMode mode)
    {
        NoteRepository.Dispose();
        NoteRepository = mode switch
        {
            StorageMode.Cloud => new CloudNoteRepository(_client, _authentication),
            _ => new StorageNoteRepository()
        };
        Mode = mode;
        Refresh?.Invoke();
    }
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                NoteRepository.Dispose();
                _client.Dispose();
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    public IRepository<NoteModel> NoteRepository { get; private set; } = new StorageNoteRepository();
    public Action? Refresh { get; set; }
    public StorageMode Mode { get; private set; } = StorageMode.Json;
    private bool _disposedValue;
    private readonly HttpClient _client;
    private readonly IBearerAuthentication _authentication;
}