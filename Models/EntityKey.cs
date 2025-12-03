namespace Ixjok.Models;

public record struct EntityKey<T>(T Value) : IComparable<EntityKey<T>>
    where T : notnull, IComparable<T>
{
    public T Value { get; init; } = Value;
    public readonly override string ToString() => Value.ToString()!;
    public readonly int CompareTo(EntityKey<T> other) => Value.CompareTo(other.Value);
    public static implicit operator T(EntityKey<T> entity) => entity.Value;
    public static implicit operator EntityKey<T>(T value) => new(value);
}