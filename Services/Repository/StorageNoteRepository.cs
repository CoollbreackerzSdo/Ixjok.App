using System.Collections.ObjectModel;
using System.Text.Json;
using Ixjok.Models.Note;

namespace Ixjok.Services.Repository;

public sealed partial class StorageNoteRepository : IJsonRepository<NoteModel>
{
    public StorageNoteRepository()
    {
        if (!File.Exists(ConfigHelper.JsonDatabasePath))
        {
            File.WriteAllText(ConfigHelper.JsonDatabasePath, "[]");
            _table = [];
            return;
        }
        _table = new(JsonSerializer.Deserialize<ObservableCollection<NoteModel>>(File.ReadAllText(ConfigHelper.JsonDatabasePath))!);
    }
    public async Task<Result> AddAsync(NoteModel model, CancellationToken token = default)
    {
        _table.Add(model);
        await File.WriteAllTextAsync(ConfigHelper.JsonDatabasePath, JsonSerializer.Serialize(_table), token);
        return Result.Success();
    }
    public IEnumerable<NoteModel> GetAll() => _table;
    public ObservableCollection<NoteModel> GetObservableAll() => _table;
    public async Task<Result> UpdateAsync(NoteModel model, CancellationToken token = default)
    {
        await File.WriteAllTextAsync(ConfigHelper.JsonDatabasePath, JsonSerializer.Serialize(_table), token);
        return Result.Success();
    }
    public async Task<Result> RemoveAsync(NoteModel model, CancellationToken token = default)
    {
        _table.Remove(model);
        await File.WriteAllTextAsync(ConfigHelper.JsonDatabasePath, JsonSerializer.Serialize(_table), token);
        return Result.Success();
    }
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _table.Clear();
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    private readonly ObservableCollection<NoteModel> _table;
    private bool _disposedValue;
}