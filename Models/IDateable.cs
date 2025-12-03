namespace Ixjok.Models;

public interface IDateable
{
    DateTimeOffset Registration { get; }
    DateOnly Date => DateOnly.FromDateTime(Registration.DateTime);
    TimeOnly Time => TimeOnly.FromDateTime(Registration.DateTime);
}