namespace Squeal.Expressions;

internal sealed record Keyword(uint Value) : Expression
{
    public static implicit operator Keyword(uint value) => new(value);
    public static implicit operator uint(Keyword value) => value.Value;
}
