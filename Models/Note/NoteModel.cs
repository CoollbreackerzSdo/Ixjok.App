
using CommunityToolkit.Mvvm.ComponentModel;

namespace Ixjok.Models.Note;

public sealed partial class NoteModel : ObservableObject, INote
{
    [ObservableProperty]
    public partial string Title { get; set; }
    [ObservableProperty]
    public partial string Content { get; set; }
    public DateTimeOffset Registration { get; init; } = DateTimeOffset.Now;
}