using Squeal.Select;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Squeal;

internal static class Sql
{
    public static ISelectStatement Parse(string sql)
    {
        var tokens = Tokenizer.Tokenize(sql);
        return SelectStatement.Parse(tokens);
    }

    public static TokenListParserResult<SelectToken, ISelectStatement> TryParse(string sql)
    {
        var tokens = Tokenizer.Tokenize(sql);
        return SelectStatement.TryParse(tokens);
    }

    public enum SelectToken
    {
        False,
        True,
        LParen,
        RParen,
        Comma,
        Count,
        Dot,
        Star,
        All,
        As,
        By,
        Distinct,
        From,
        Group,
        Having,
        Identifier,
        Limit,
        Offset,
        Order,
        Select,
        Values,
        Where,
        Window,
    }

    internal static readonly Tokenizer<SelectToken> Tokenizer = new TokenizerBuilder<SelectToken>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('('), SelectToken.LParen)
        .Match(Character.EqualTo(')'), SelectToken.RParen)
        .Match(Character.EqualTo(','), SelectToken.Comma)
        .Match(Character.EqualTo('.'), SelectToken.Dot)
        .Match(Character.EqualTo('*'), SelectToken.Star)
        .Match(Span.EqualToIgnoreCase("ALL"), SelectToken.All, true)
        .Match(Span.EqualToIgnoreCase("AS"), SelectToken.As, true)
        .Match(Span.EqualToIgnoreCase("BY"), SelectToken.By, true)
        .Match(Span.EqualToIgnoreCase("COUNT"), SelectToken.Count, true)
        .Match(Span.EqualToIgnoreCase("DISTINCT"), SelectToken.Distinct, true)
        .Match(Span.EqualToIgnoreCase("FROM"), SelectToken.From, true)
        .Match(Span.EqualToIgnoreCase("GROUP"), SelectToken.Group, true)
        .Match(Span.EqualToIgnoreCase("HAVING"), SelectToken.Having, true)
        .Match(Span.EqualToIgnoreCase("LIMIT"), SelectToken.Limit, true)
        .Match(Span.EqualToIgnoreCase("OFFSET"), SelectToken.Offset, true)
        .Match(Span.EqualToIgnoreCase("ORDER"), SelectToken.Order, true)
        .Match(Span.EqualToIgnoreCase("SELECT"), SelectToken.Select, true)
        .Match(Span.EqualToIgnoreCase("TRUE"), SelectToken.True, true)
        .Match(Span.EqualToIgnoreCase("FALSE"), SelectToken.False, true)
        .Match(Span.EqualToIgnoreCase("VALUES"), SelectToken.Values, true)
        .Match(Span.EqualToIgnoreCase("WHERE"), SelectToken.Where, true)
        .Match(Span.EqualToIgnoreCase("WINDOW"), SelectToken.Window, true)
        .Match(Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), SelectToken.Identifier, true)
        .Build();

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> Comma =
        Token.EqualTo(SelectToken.Comma);

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> LParen =
        Token.EqualTo(SelectToken.LParen);

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> RParen =
        Token.EqualTo(SelectToken.RParen);

    internal static readonly TokenListParser<SelectToken, bool> HasDot =
        Token.EqualTo(SelectToken.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> Identifier =
        Token.EqualTo(SelectToken.Identifier);

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> Star =
        Token.EqualTo(SelectToken.Star);

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> Distinct =
        Token.EqualTo(SelectToken.Distinct);

    internal static readonly TokenListParser<SelectToken, TableName> TableName =
        Identifier.Apply(Squeal.Parse.AsString)
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(Squeal.Parse.AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier, firstIdentifier)
            : new TableName(firstIdentifier, null))));

    internal static readonly TokenListParser<SelectToken, TableName> From =
        Token.EqualTo(SelectToken.From)
        .IgnoreThen(TableName);

    internal static readonly TokenListParser<SelectToken, ResultColumn> ResultColumn =
        Identifier.Apply(Squeal.Parse.AsString)
        .Or(Star.Apply(Squeal.Parse.AsString))
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(Squeal.Parse.AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new ResultColumn(secondIdentifier, firstIdentifier)
            : new ResultColumn(firstIdentifier, null))));

    internal static readonly TokenListParser<SelectToken, ResultColumn[]> Columns =
        ResultColumn.ManyDelimitedBy(Comma)
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<SelectToken, Token<SelectToken>> Count =
        Token.EqualTo(SelectToken.Count)
        .IgnoreThen(LParen)
        .IgnoreThen(Star)
        .IgnoreThen(RParen);

    internal static readonly TokenListParser<SelectToken, ISelectStatement> SelectStatement =
        Token.EqualTo(SelectToken.Select)
        .IgnoreThen(Columns
            .Then(columns => From
                .Select(table => (ISelectStatement)new SelectStatement(table, columns)))
            .Or(Count.IgnoreThen(From
                .Select(table => (ISelectStatement)new SelectCountStatement(table)))));
}
