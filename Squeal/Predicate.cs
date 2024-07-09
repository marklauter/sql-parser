namespace Squeal;

public record Predicate(Expression Expression)
{
    public static Predicate Default { get; } = new(Expression.Default);

    public static Predicate Create(Expression expression) =>
        new(expression);
}
