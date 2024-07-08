namespace Squeal.Select.Expressions;

public record ConditionalExpression(Expression Left, Expression Right, ConditionalOperators Operator)
    : BinaryExpression(Left, Right);
