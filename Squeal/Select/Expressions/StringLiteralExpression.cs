namespace Squeal.Select.Expressions;

public record StringLiteralExpression(string Value)
    : Expression
{
    public static StringLiteralExpression Create(string value) => new(value);
}
