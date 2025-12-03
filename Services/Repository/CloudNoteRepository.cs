using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;
using Ixjok.Models.Note;
using Ixjok.Services.Auth;

namespace Ixjok.Services.Repository;

public sealed partial class CloudNoteRepository(HttpClient client, IBearerAuthentication authentication) : IRepository<NoteModel>
{
    public ObservableCollection<NoteModel> GetObservableAll()
    {
        if (!authentication.RequestAuthenticationStateAsync().GetAwaiter().GetResult())
            return Notes;
        client.DefaultRequestHeaders.Authorization = new("bearer", authentication.Token);
        var result = client.GetAsync("notes").GetAwaiter().GetResult();
        if (result.IsSuccessStatusCode)
        {
            Notes = result.Content.ReadFromJsonAsync<ObservableCollection<NoteModel>>().GetAwaiter().GetResult() ?? [];
        }
        return Notes;
    }
    public IEnumerable<NoteModel> GetAll()
    {
        if (!authentication.RequestAuthenticationStateAsync().GetAwaiter().GetResult())
            return Notes;
        client.DefaultRequestHeaders.Authorization = new("bearer", authentication.Token);
        var result = client.GetAsync("notes").GetAwaiter().GetResult();
        if (result.IsSuccessStatusCode)
        {
            Notes = result.Content.ReadFromJsonAsync<ObservableCollection<NoteModel>>().GetAwaiter().GetResult() ?? [];
        }
        return Notes;
    }
    public async Task<Result> AddAsync(NoteModel model, CancellationToken token = default)
    {
        client.DefaultRequestHeaders.Authorization = new("bearer", authentication.Token);
        var result = await client.PostAsJsonAsync("notes", model, token);
        if (result.IsSuccessStatusCode)
        {
            Notes.Add(model);
            return Result.Success();
        }
        return result.StatusCode switch
        {
            HttpStatusCode.NotFound => Result.NoFound(),
            HttpStatusCode.BadRequest => Result.BadRequest(),
            _ => Result.TeaBreak()
        };
    }
    public async Task<Result> RemoveAsync(NoteModel model, CancellationToken token = default)
    {
        client.DefaultRequestHeaders.Authorization = new("bearer", authentication.Token);
        var result = await client.PostAsJsonAsync("notes/delete", new
        {
            model.Registration
        }, token);
        if (result.IsSuccessStatusCode)
        {
            Notes.Remove(model);
            return Result.Success();
        }
        return result.StatusCode switch
        {
            HttpStatusCode.NotFound => Result.NoFound(),
            HttpStatusCode.BadRequest => Result.BadRequest(),
            _ => Result.TeaBreak()
        };
    }
    public Task<Result> UpdateAsync(NoteModel model, CancellationToken token = default)
    {
        // client.DefaultRequestHeaders.Authorization = new("bearer", authentication.Token);
        return Task.FromResult(Result.Success());
    }
    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                Notes.Clear();
            }
            _disposedValue = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    private bool _disposedValue;
    private ObservableCollection<NoteModel> Notes { get; set; } = [];
}