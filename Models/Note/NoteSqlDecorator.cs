using SQLite;

namespace Ixjok.Models.Note;

[Table("notes")]
public sealed class NoteSqlDecorator
{
    [Column("id"), PrimaryKey]
    public string? Key { get; set; } = Guid.CreateVersion7().ToString();
    [Column("title"), Indexed]
    public string? Title { get; set; } 
    [Column("content")]
    public string? Content { get; set; } 
    [Column("registration")]
    public string? Registration { get; set; } 
    [Column("status")]
    public int Status { get; set; } = (int)NoteState.Local;
}