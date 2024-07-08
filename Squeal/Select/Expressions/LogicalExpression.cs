namespace Squeal.Select.Expressions;

public record LogicalExpression(Expression Left, Expression Right, LogicalOperators Operator)
    : BinaryExpression(Left, Right);
