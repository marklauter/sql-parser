using Lexi;

namespace Squeal.Expressions;

internal sealed record Identifier(string Value) : Expression
{
    public static implicit operator Identifier(string value) => new(value);
    public static implicit operator string(Identifier value) => value.Value;

    public static Identifier FromMatch(MatchResult match) => match.Source.ReadSymbol(in match.Symbol);
}
