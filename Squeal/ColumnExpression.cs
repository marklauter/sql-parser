namespace Squeal;

public record ColumnExpression(string ColumnName, string? TableName, string? SchemaName)
    : Expression
{
    public static ColumnExpression Create(string ColumnName) => new(ColumnName, null, null);
}
