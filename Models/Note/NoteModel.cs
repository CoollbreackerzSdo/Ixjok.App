using System.Text.Json.Serialization;
using Ixjok.Tools.Converts;

namespace Ixjok.Models.Note;

public sealed partial class NoteModel : ObservableObject, INote
{
    [JsonConverter(typeof(KeyJsonConvert))]
    public KeyId Id { get; set; } = Guid.CreateVersion7();
    [ObservableProperty]
    public partial string Title { get; set; }
    [ObservableProperty]
    public partial string Content { get; set; }
    public DateTimeOffset Registration { get; init; } = DateTimeOffset.Now;
}