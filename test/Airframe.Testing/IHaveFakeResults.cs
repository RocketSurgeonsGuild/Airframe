namespace Airframe.Testing;

public interface IHaveFakeResults<T>
{
    List<Result<T>> Results { get; }
}

public record Result<T>(T Value)
{
    public T Value { get; } = Value;
}