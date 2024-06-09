using Lexi;
using System.Text.RegularExpressions;
using Squeal.CreateStatement;
using Squeal.Expressions;

namespace Squeal;

internal static class SqlParser
{
    private static readonly Lexer Lexer = VocabularyBuilder
        .Create(RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
        .Match(nameof(StatementTokens.Create), (uint)StatementTokens.Create)
        .Match(nameof(StatementTokens.Select), (uint)StatementTokens.Select)
        .Match(nameof(StatementTokens.Alter), (uint)StatementTokens.Alter)
        .Match(nameof(StatementTokens.Drop), (uint)StatementTokens.Drop)
        .Match(nameof(StatementTokens.Insert), (uint)StatementTokens.Insert)
        .Match(nameof(StatementTokens.Update), (uint)StatementTokens.Update)
        .Match(nameof(StatementTokens.Delete), (uint)StatementTokens.Delete)
        .Ignore(CommonPatterns.NewLine(), (uint)StatementTokens.Ignore)
        .Ignore(CommonPatterns.Whitespace(), (uint)StatementTokens.Ignore)
        .Build();

    public static Statement Parse(string sql)
    {
        var keyword = Lexer.NextMatch(sql);
        return keyword.Symbol.IsMatch
            ? ParseKeyword(keyword)
            : throw new InvalidOperationException($"unexpected token {keyword.Symbol.TokenId} @ {keyword.Symbol.Offset}");
    }

    public static TStatement Parse<TStatement>(string sql)
        where TStatement : Statement
    {
        var keyword = Lexer.NextMatch(sql);
        return keyword.Symbol.IsMatch
            ? (TStatement)ParseKeyword(keyword)
            : throw new InvalidOperationException($"unexpected token {keyword.Symbol.TokenId} @ {keyword.Symbol.Offset}");
    }

    private static Statement ParseKeyword(MatchResult matchResult) => (StatementTokens)matchResult.Symbol.TokenId switch
    {
        StatementTokens.Create => CreateStatementParser.Parse(matchResult),
        StatementTokens.Select or
        StatementTokens.Alter or
        StatementTokens.Drop or
        StatementTokens.Insert or
        StatementTokens.Update or
        StatementTokens.Delete => throw new NotImplementedException(),
        _ => throw new InvalidOperationException($"unexpected token {(StatementTokens)matchResult.Symbol.TokenId}"),
    };
}
