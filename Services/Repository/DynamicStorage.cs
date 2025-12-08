using System.Collections.ObjectModel;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;
using SQLite;

namespace Ixjok.Services.Repository;

public sealed partial class DynamicStorage : IDynamicStorage
{
    public DynamicStorage(IHostedNoteRepository hostedNoteRepository, IBearerAuthenticationHandler authentication)
    {
        _hostedNoteRepository = hostedNoteRepository;
        _authentication = authentication;
        _authentication.AuthenticationChange += Connectivity;
        InitAsync().ConfigureAwait(false);
    }

    private async void Connectivity(AuthenticationState state) => _ = state switch
    {
        AuthenticationState.Connected => ConnectToHostAsync(),
        _ => DesConnectToHostAsync()
    };

    private async Task InitAsync()
    {
        if (_connection is not null) return;
        _connection = new(ConfigHelper.SqlDatabasePath, ConfigHelper.SqlFlags);
        var result = await _connection.CreateTableAsync<NoteSqlDecorator>();
        if (result == CreateTableResult.Migrated)
        {
            Notes = new(_connection!.Table<NoteSqlDecorator>().Where(x => x.Status != (int)NoteState.Deleted).OrderBy(x => x.Registration).ToArrayAsync().GetAwaiter().GetResult().Select(x => x.ToModel()));
        }
    }
    public async Task RemoveAsync(NoteModel model, CancellationToken token = default)
    {
        // await InitAsync();
        var deco = model.ToDecorator();
        if (await _authentication.AuthenticationStateAsync(token) && (await _hostedNoteRepository.DeleteAsync(model.Id!.Value, token)).IsSuccess)
            deco.Status = (int)NoteState.Cloud;
        _ = _connection!.DeleteAsync(model.ToDecorator());
        Notes.Remove(model);
    }
    public async Task AddAsync(NoteModel model, CancellationToken token = default)
    {
        // await InitAsync();
        var deco = model.ToDecorator();
        if (await _authentication.AuthenticationStateAsync(token) && (await _hostedNoteRepository.AddAsync(model, token)).IsSuccess)
            deco.Status = (int)NoteState.Cloud;
        _ = _connection!.InsertAsync(deco);
        Notes.Add(model);
    }
    public ObservableCollection<NoteModel> GetObservableAll()
    {
        InitAsync().Wait();
        return Notes;
    }

    public async Task UpdateAsync(NoteModel model, CancellationToken token = default)
    {
        // await InitAsync();
        var deco = model.ToDecorator();
        deco.Status = (int)NoteState.Update;
        if (await _authentication.AuthenticationStateAsync(token) && (await _hostedNoteRepository.UpdateAsync(model, token)).IsSuccess)
            deco.Status = (int)NoteState.Cloud;
        _ = _connection!.UpdateAsync(deco);
    }
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Notes.Clear();
#pragma warning disable CS8601 // Posible asignación de referencia nula
                _authentication.AuthenticationChange -= Connectivity;
#pragma warning restore CS8601 // Posible asignación de referencia nula
            }
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    private async Task<Result> DesConnectToHostAsync(CancellationToken token = default)
    {
        Notes.Clear();
        await _connection!.DeleteAllAsync<NoteSqlDecorator>();
        return Result.Success();
    }
    public async Task<Result> ConnectToHostAsync(CancellationToken token = default)
    {
        Notes.Clear();
        await foreach (var item in _hostedNoteRepository.GetAllAsync(token))
        {
            var deco = item.ToDecorator();
            deco.Status = (int)NoteState.Cloud;
            _ = _connection!.InsertAsync(deco);
            Notes.Add(item);
        }
        return Result.Success();
    }
    private ObservableCollection<NoteModel> Notes { get; set; } = [];
    private SQLiteAsyncConnection? _connection;
    private readonly bool _disposedValue = false;
    private readonly IHostedNoteRepository _hostedNoteRepository;
    private readonly IBearerAuthenticationHandler _authentication;
}