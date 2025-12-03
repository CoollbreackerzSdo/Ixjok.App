namespace Ixjok.Models.Note;

public interface INote : IDateable
{
    string Title { get; set; }
    string Content { get; set; }
}