namespace Squeal;

public record LogicalExpression(LogicalOperators Operator, Expression Left, Expression Right)
    : BinaryExpression(Left, Right)
{
    public static Expression Create(LogicalOperators @operator, Expression left, Expression right) =>
        new LogicalExpression(@operator, left, right);
}
