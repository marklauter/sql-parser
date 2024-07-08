namespace Squeal.Select.Expressions;

public record NumericLiteralExpression(double Value)
    : Expression
{
    public static NumericLiteralExpression Create(double value) => new(value);
}
