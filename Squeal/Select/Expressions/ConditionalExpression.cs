namespace Squeal.Select.Expressions;

public record ConditionalExpression(ConditionalOperators Operator, Expression Left, Expression Right)
    : BinaryExpression(Left, Right)
{
    public static Expression Create(ConditionalOperators @operator, Expression left, Expression right) =>
        new ConditionalExpression(@operator, left, right);
}
