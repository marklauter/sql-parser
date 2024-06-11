using Squeal.CreateStatement;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Squeal;

public static class Ddl
{
    internal static readonly TokenListParser<SqlToken, Token<SqlToken>> Comma =
        Token.EqualTo(SqlToken.Comma);

    internal static readonly TokenListParser<SqlToken, Token<SqlToken>> LParen =
        Token.EqualTo(SqlToken.LParen);

    internal static readonly TokenListParser<SqlToken, Token<SqlToken>> RParen =
        Token.EqualTo(SqlToken.RParen);

    internal static readonly TokenListParser<SqlToken, Token<SqlToken>> Identifier =
        Token.EqualTo(SqlToken.Identifier);

    internal static readonly TextParser<string> AsString = input =>
        Result.Value(input.ToString(), input, input.Skip(input.Length));

    private static readonly TokenListParser<SqlToken, bool> IsTemporary =
        Token.EqualTo(SqlToken.IsTemporary).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, bool> HasDot =
        Token.EqualTo(SqlToken.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, int> SignedNumber =
        Token.EqualTo(SqlToken.SignedNumber).Apply(Numerics.IntegerInt32);

    private static readonly TokenListParser<SqlToken, bool> IfNotExists =
        Token.EqualTo(SqlToken.If)
        .IgnoreThen(Token.EqualTo(SqlToken.Not))
        .IgnoreThen(Token.EqualTo(SqlToken.Exists))
        .Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, TableName> TableName =
        Identifier.Apply(AsString)
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier, firstIdentifier)
            : new TableName(firstIdentifier, null))));

    internal static readonly TokenListParser<SqlToken, string> ColumnType =
        Token.EqualTo(SqlToken.ColumnTypeBlob)
        .Or(Token.EqualTo(SqlToken.ColumnTypeInt))
        .Or(Token.EqualTo(SqlToken.ColumnTypeNumeric))
        .Or(Token.EqualTo(SqlToken.ColumnTypeReal))
        .Or(Token.EqualTo(SqlToken.ColumnTypeText))
        .Apply(AsString)
        .OptionalOrDefault("BLOB");

    internal static readonly TokenListParser<SqlToken, int[]> TypeModifier =
        LParen.Optional()
        .IgnoreThen(SignedNumber.ManyDelimitedBy(Comma)
        .Then(numbers => RParen.Value(numbers)))
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<SqlToken, TypeName> TypeName =
        ColumnType
        .Then(name => TypeModifier
        .Select(signedNumbers => new TypeName(name, signedNumbers)));

    internal static readonly TokenListParser<SqlToken, ColumnDef> Column =
        Identifier.Apply(AsString)
        .Then(name => TypeName
        .Select(type => new ColumnDef(name, type, null)));

    //internal static TextParser<ColumnDef[]> Columns { get; } =
    //    Span.WhiteSpace.Optional()
    //    .IgnoreThen(OpenParen)
    //    .IgnoreThen(Column.ManyDelimitedBy(Comma))
    //    .Then(columns => CloseParen.Value(columns));

    public static readonly TokenListParser<SqlToken, CreateTableStatement> CreateTableStatement =
        Token.EqualTo(SqlToken.Create)
        .IgnoreThen(IsTemporary)
        .Then(isTemp => Token.EqualTo(SqlToken.Table)
        .IgnoreThen(IfNotExists)
        .Then(ifNotExists => TableName
        .Select(name => new CreateTableStatement(name, isTemp, ifNotExists))));
}
