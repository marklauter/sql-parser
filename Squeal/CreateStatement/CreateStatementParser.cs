using Lexi;
using Squeal.CreateStatement.Expressions;
using Squeal.Expressions;
using System.Text.RegularExpressions;

namespace Squeal.CreateStatement;

// does not support table options, table constraint, or column constraint
internal static class CreateStatementParser
{
    private static readonly Lexer Lexer = VocabularyBuilder
        .Create(RegexOptions.IgnoreCase)
        .Match("temporary|temp", (uint)CreateStatementTokens.Temporary)
        .Match(nameof(CreateStatementTokens.Table), (uint)CreateStatementTokens.Table)
        .Match("if", (uint)CreateStatementTokens.If)
        .Match("not", (uint)CreateStatementTokens.Not)
        .Match("exists", (uint)CreateStatementTokens.Exists)
        .Match(@"\.", (uint)CreateStatementTokens.Dot)
        .Match(@"\(", (uint)CreateStatementTokens.OpenParens)
        .Match(@"\)", (uint)CreateStatementTokens.CloseParens)
        .Match(",", (uint)CreateStatementTokens.Comma)
        .Match("text", (uint)CreateStatementTokens.ColumnTypeText)
        .Match("numeric|num", (uint)CreateStatementTokens.ColumnTypeNumeric)
        .Match("integer|int", (uint)CreateStatementTokens.ColumnTypeInteger)
        .Match("real", (uint)CreateStatementTokens.ColumnTypeReal)
        .Match("blob|none", (uint)CreateStatementTokens.ColumnTypeBlob)
        .Match(CommonPatterns.Identifier(), (uint)CreateStatementTokens.Identifier)
        .Ignore(CommonPatterns.NewLine(), (uint)CreateStatementTokens.Ignore)
        .Ignore(CommonPatterns.Whitespace(), (uint)CreateStatementTokens.Ignore)
        .Build();

    /// <summary>
    /// https://www.sqlite.org/lang_createtable.html
    /// </summary>
    /// <param name="lastMatch"><see cref="MatchResult"/></param>
    /// <returns><see cref="CreateTableStatement"/></returns>
    public static CreateTableStatement Parse(MatchResult lastMatch)
    {
        var tableResult = ParseTable(lastMatch);
        var columnsResult = ParseColumns(tableResult.MatchResult);

        return new CreateTableStatement(
            tableResult.Result!,
            columnsResult.Result!);
    }

    private static ParseResult<Table> ParseTable(MatchResult lastMatch)
    {
        // next is temp|temporary|table 
        var tableKindToken = Lexer.NextMatch(lastMatch);
        var tableKind = tableKindToken.Symbol.TokenId switch
        {
            (uint)CreateStatementTokens.Temporary => TableKind.Temporary,
            (uint)CreateStatementTokens.Table => TableKind.Default,
            _ => throw new InvalidOperationException($"unexpected token {tableKindToken.Symbol.TokenId} @ {tableKindToken.Symbol.Offset}")
        };

        var result = ParseTableName(tableKindToken);
        var table = new Table(result.Result.schema, result.Result.name, tableKind);

        return new(table, result.MatchResult);
    }

    private static ParseResult<(Identifier? schema, Identifier name)> ParseTableName(MatchResult lastMatch)
    {
        Identifier? schema = null;
        var identifier = Lexer.NextMatch(lastMatch);
        if (!identifier.Symbol.IsMatch || identifier.Symbol.TokenId != (uint)CreateStatementTokens.Identifier)
        {
            throw new InvalidOperationException($"unexpected token {identifier.Symbol.TokenId} @ {identifier.Symbol.Offset}");
        }

        var dot = Lexer.NextMatch(identifier);
        if (dot.Symbol.IsMatch && dot.Symbol.TokenId == (uint)CreateStatementTokens.Dot)
        {
            schema = Identifier.FromMatch(identifier);

            identifier = Lexer.NextMatch(dot);
            if (!identifier.Symbol.IsMatch || identifier.Symbol.TokenId != (uint)CreateStatementTokens.Identifier)
            {
                throw new InvalidOperationException($"unexpected token {identifier.Symbol.TokenId} @ {identifier.Symbol.Offset}");
            }
        }

        var name = Identifier.FromMatch(identifier);
        return new((schema, name), identifier);
    }

    private static ParseResult<ColumnDef> ParseColumns(MatchResult lastMatch)
    {
        var openParens = NextMatch(lastMatch, (uint)CreateStatementTokens.OpenParens);
        var columnResult = ParseColumn(openParens);
        return new ParseResult<ColumnDef>(columnResult.Result, columnResult.MatchResult);
    }

    // todo: handle column constraint - https://www.sqlite.org/syntax/column-def.html and https://www.sqlite.org/syntax/column-constraint.html
    private static ParseResult<ColumnDef?> ParseColumn(MatchResult lastMatch)
    {
        var nextMatch = Lexer.NextMatch(lastMatch);
        if (nextMatch.Symbol.IsMatch && nextMatch.Symbol.TokenId == (uint)CreateStatementTokens.CloseParens)
        {
            return new ParseResult<ColumnDef?>(null, nextMatch);
        }

        var columnName = new Identifier(nextMatch.Source.ReadSymbol(in nextMatch.Symbol));
        nextMatch = NextMatch(nextMatch, IsColumnType);
        var columnType = ColumnType.FromToken(nextMatch.Symbol.TokenId);

        var nextColumn = ParseColumn(nextMatch);

        return new(
            new(columnName, columnType, nextColumn.Result),
            nextColumn.MatchResult);
    }

    private static MatchResult NextMatch(MatchResult lastMatch, uint expectedToken)
    {
        var nextMatch = Lexer.NextMatch(lastMatch);
        return !nextMatch.Symbol.IsMatch || nextMatch.Symbol.TokenId != expectedToken
            ? throw new InvalidOperationException($"unexpected token {nextMatch.Symbol.TokenId} @ {nextMatch.Symbol.Offset}, expected {expectedToken}")
            : nextMatch;
    }

    private static MatchResult NextMatch(MatchResult lastMatch, Func<uint, bool> isValid)
    {
        var nextMatch = Lexer.NextMatch(lastMatch);
        return !nextMatch.Symbol.IsMatch || isValid(nextMatch.Symbol.TokenId)
            ? throw new InvalidOperationException($"unexpected token {nextMatch.Symbol.TokenId} @ {nextMatch.Symbol.Offset}")
            : nextMatch;
    }

    private static bool IsColumnType(uint token) => (CreateStatementTokens)token switch
    {
        CreateStatementTokens.ColumnTypeText or
        CreateStatementTokens.ColumnTypeNumeric or
        CreateStatementTokens.ColumnTypeInteger or
        CreateStatementTokens.ColumnTypeReal or
        CreateStatementTokens.ColumnTypeBlob => true,
        _ => false,
    };
}
