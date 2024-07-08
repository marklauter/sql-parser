namespace Squeal.Select.Expressions;

public record BinaryExpression(Expression Left, Expression Right)
    : Expression;
