namespace Squeal;

public record BinaryExpression(Expression Left, Expression Right)
    : Expression;
