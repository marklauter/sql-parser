using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Squeal;

internal static class Sql
{
    public static ISelectStatement Parse(string sql) =>
        SelectStatement.Parse(Tokenizer.Tokenize(sql));

    public static TokenListParserResult<SelectTokens, ISelectStatement> TryParse(string sql) =>
        SelectStatement.TryParse(Tokenizer.Tokenize(sql));

    public enum SelectTokens
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
        StringLiteral,
        // binary operators
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanOrEqualTo,
        LessThan,
        LessThanOrEqualTo,
        Or,
        And,
        // 
    }

    internal static readonly Tokenizer<SelectTokens> Tokenizer = new TokenizerBuilder<SelectTokens>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('('), SelectTokens.LParen)
        .Match(Character.EqualTo(')'), SelectTokens.RParen)
        .Match(Character.EqualTo('*'), SelectTokens.Star, true)
        .Match(Character.EqualTo(','), SelectTokens.Comma)
        .Match(Character.EqualTo('.'), SelectTokens.Dot)
        .Match(Character.EqualTo('<'), SelectTokens.LessThan, true)
        .Match(Character.EqualTo('='), SelectTokens.EqualTo, true)
        .Match(Character.EqualTo('>'), SelectTokens.GreaterThan, true)
        .Match(Span.EqualTo("!="), SelectTokens.NotEqualTo, true)
        .Match(Span.EqualTo("<="), SelectTokens.LessThanOrEqualTo, true)
        .Match(Span.EqualTo("<>"), SelectTokens.NotEqualTo, true)
        .Match(Span.EqualTo(">="), SelectTokens.GreaterThanOrEqualTo, true)
        .Match(Span.EqualToIgnoreCase("ALL"), SelectTokens.All, true)
        .Match(Span.EqualToIgnoreCase("AND"), SelectTokens.And, true)
        .Match(Span.EqualToIgnoreCase("AS"), SelectTokens.As, true)
        .Match(Span.EqualToIgnoreCase("BY"), SelectTokens.By, true)
        .Match(Span.EqualToIgnoreCase("COUNT"), SelectTokens.Count, true)
        .Match(Span.EqualToIgnoreCase("DISTINCT"), SelectTokens.Distinct, true)
        .Match(Span.EqualToIgnoreCase("FALSE"), SelectTokens.False, true)
        .Match(Span.EqualToIgnoreCase("FROM"), SelectTokens.From, true)
        .Match(Span.EqualToIgnoreCase("GROUP"), SelectTokens.Group, true)
        .Match(Span.EqualToIgnoreCase("HAVING"), SelectTokens.Having, true)
        .Match(Span.EqualToIgnoreCase("LIMIT"), SelectTokens.Limit, true)
        .Match(Span.EqualToIgnoreCase("OFFSET"), SelectTokens.Offset, true)
        .Match(Span.EqualToIgnoreCase("OR"), SelectTokens.Or, true)
        .Match(Span.EqualToIgnoreCase("ORDER"), SelectTokens.Order, true)
        .Match(Span.EqualToIgnoreCase("SELECT"), SelectTokens.Select, true)
        .Match(Span.EqualToIgnoreCase("TRUE"), SelectTokens.True, true)
        .Match(Span.EqualToIgnoreCase("VALUES"), SelectTokens.Values, true)
        .Match(Span.EqualToIgnoreCase("WHERE"), SelectTokens.Where, true)
        .Match(Span.EqualToIgnoreCase("WINDOW"), SelectTokens.Window, true)
        .Match(Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), SelectTokens.Identifier, true)
        .Match(Span.Regex(@"\G'(?:[^'\\\n\r]|\\.)*'"), SelectTokens.StringLiteral, true)
        .Build();

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> Comma =
        Token.EqualTo(SelectTokens.Comma);

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> LParen =
        Token.EqualTo(SelectTokens.LParen);

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> RParen =
        Token.EqualTo(SelectTokens.RParen);

    internal static readonly TokenListParser<SelectTokens, bool> HasDot =
        Token.EqualTo(SelectTokens.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> Identifier =
        Token.EqualTo(SelectTokens.Identifier);

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> Star =
        Token.EqualTo(SelectTokens.Star);

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> Distinct =
        Token.EqualTo(SelectTokens.Distinct);

    internal static readonly TokenListParser<SelectTokens, TableName> TableName =
        Identifier.Apply(Value.AsString)
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(Value.AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier, firstIdentifier)
            : new TableName(firstIdentifier, null))));

    internal static readonly TokenListParser<SelectTokens, TableName> From =
        Token.EqualTo(SelectTokens.From)
        .IgnoreThen(TableName);

    internal static readonly TokenListParser<SelectTokens, ProjectedColumn> ProjectedColumn =
        Identifier.Apply(Value.AsString)
        .Or(Star.Apply(Value.AsString))
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(Value.AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new ProjectedColumn(secondIdentifier, firstIdentifier, null)
            : new ProjectedColumn(firstIdentifier, null, null))));

    internal static readonly TokenListParser<SelectTokens, ProjectedColumn[]> Projection =
        ProjectedColumn.ManyDelimitedBy(Comma)
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<SelectTokens, ConditionalOperators> BinaryOperator =
        Token.EqualTo(SelectTokens.EqualTo).Value(ConditionalOperators.EqualTo)
        .Or(Token.EqualTo(SelectTokens.NotEqualTo).Value(ConditionalOperators.NotEqualTo))
        .Or(Token.EqualTo(SelectTokens.GreaterThan).Value(ConditionalOperators.GreaterThan))
        .Or(Token.EqualTo(SelectTokens.GreaterThanOrEqualTo).Value(ConditionalOperators.GreaterThanOrEqualTo))
        .Or(Token.EqualTo(SelectTokens.LessThan).Value(ConditionalOperators.LessThan))
        .Or(Token.EqualTo(SelectTokens.LessThanOrEqualTo).Value(ConditionalOperators.LessThanOrEqualTo));

    internal static readonly TokenListParser<SelectTokens, LogicalOperators> LogicalOperator =
        Token.EqualTo(SelectTokens.Or).Value(LogicalOperators.Or)
        .Or(Token.EqualTo(SelectTokens.And).Value(LogicalOperators.And));

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> StringLiteral =
        Token.EqualTo(SelectTokens.StringLiteral);

    internal static readonly TokenListParser<SelectTokens, ColumnExpression> ColumnExp =
        Identifier
            .Apply(Value.AsString)
            .Select(ColumnExpression.Create);

    internal static readonly TokenListParser<SelectTokens, StringLiteralExpression> LiteralExp =
        StringLiteral
            .Apply(Value.AsString)
            .Select(StringLiteralExpression.Create);

    internal static readonly TokenListParser<SelectTokens, Expression> ConditionalOperand =
        ColumnExp.Select(e => (Expression)e)
        .Or(LiteralExp.Select(e => (Expression)e));

    internal static readonly TokenListParser<SelectTokens, Expression> ConditionalExp =
        Superpower.Parse.Chain(BinaryOperator, ConditionalOperand, ConditionalExpression.Create);

    internal static readonly TokenListParser<SelectTokens, Expression> LogicalExp =
        Superpower.Parse.Chain(LogicalOperator, ConditionalExp, LogicalExpression.Create);

    internal static readonly TokenListParser<SelectTokens, Expression> ExpressionTree =
        LogicalExp.Or(ConditionalExp);

    internal static readonly TokenListParser<SelectTokens, Predicate> Where =
        Token.EqualTo(SelectTokens.Where)
        .IgnoreThen(ExpressionTree.Select(Predicate.Create))
        .OptionalOrDefault(Predicate.Default);

    internal static readonly TokenListParser<SelectTokens, Token<SelectTokens>> Count =
        Token.EqualTo(SelectTokens.Count)
        .IgnoreThen(LParen)
        .IgnoreThen(Star)
        .IgnoreThen(RParen);

    internal static readonly TokenListParser<SelectTokens, ISelectStatement> SelectStatement =
        Token.EqualTo(SelectTokens.Select)
        .IgnoreThen(Projection
            .Then(columns => From
            .Then(table => Where
                .Select(where => (ISelectStatement)new SelectStatement(table, columns, where)))
            .Or(Count.IgnoreThen(From
                .Then(table => Where
                .Select(where => (ISelectStatement)new SelectCountStatement(table, where)))))));
}
