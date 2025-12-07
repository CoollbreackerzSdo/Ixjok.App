using System.Net;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Ixjok.Models.Note;
using Ixjok.Tools.Common.Models.Page;

namespace Ixjok.Services.Repository;

public sealed partial class HostedNoteStorage(HttpClient client) : IHostedNoteRepository
{
    public async Task<Result> AddAsync(NoteModel model, CancellationToken token = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return Result.Canceled();

        var result = await _client.PostAsJsonAsync("notes/complex", new
        {
            id = model.Id.Value,
            title = model.Title,
            content = model.Content,
            registration = model.Registration
        }, token);
        return result.StatusCode switch
        {
            HttpStatusCode.OK => Result.Success(),
            HttpStatusCode.BadRequest => Result.BadRequest(),
            HttpStatusCode.NotFound => Result.NoFound(),
            HttpStatusCode.Unauthorized => Result.UnAuthorized(),
            _ => Result.TeaBreak()
        };
    }
    public async IAsyncEnumerable<NoteModel> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            yield break;

        var query = new PageRequest(0, 50, PageOrder.Asc);
        var isNotComplete = true;
        while (isNotComplete)
        {
            var result = await _client.PostAsJsonAsync("notes/page", query, cancellationToken);
            if (!result.IsSuccessStatusCode) break;

            var currentIterations = 0;
            await foreach (var item in result.Content.ReadFromJsonAsAsyncEnumerable<NoteModel>(cancellationToken))
            {
                if (item is null)
                {
                    isNotComplete = false;
                    break;
                }
                currentIterations++;
                yield return item;
            }
            if (currentIterations < query.Take)
                break;
            query = query with { Skip = query.Skip + 50 };
            continue;
        }
    }
    public async Task<Result> DeleteAsync(Guid id, CancellationToken token = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return Result.Canceled();
        var result = await _client.DeleteAsync($"notes/{id}", token);
        return result.IsSuccessStatusCode || result.StatusCode == HttpStatusCode.NotFound ? Result.Success() : Result.NoFound();
    }
    public async Task<Result> UpdateAsync(NoteModel model, CancellationToken token = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return Result.Canceled();
        var result = await _client.PutAsJsonAsync($"notes", new { Id = model.Id.Value, model.Title, model.Content }, token);
        return result.IsSuccessStatusCode ? Result.Success() : Result.NoContent();
    }
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!_isDisposing)
            {

            }
            _isDisposing = true;
        }
    }
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    private bool _isDisposing = false;
    private readonly HttpClient _client = client;
}