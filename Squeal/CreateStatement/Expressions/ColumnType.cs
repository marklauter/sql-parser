using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record ColumnType(ColumnTypes Value) : Expression
{
    public static ColumnType FromToken(uint token) => new((ColumnTypes)token);
}
