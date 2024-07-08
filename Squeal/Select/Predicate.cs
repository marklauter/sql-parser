using Squeal.Select.Expressions;

namespace Squeal.Select;

public record Predicate(Expression Expression)
{
    public static Predicate Default { get; } = new(Expression.Default);
}
