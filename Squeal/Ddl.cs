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

    private static readonly TokenListParser<SqlToken, bool> IsTemporary =
        Token.EqualTo(SqlToken.IsTemporary).Value(true).OptionalOrDefault(false);

    private static readonly TokenListParser<SqlToken, bool> IfNotExists =
        Token.EqualTo(SqlToken.If)
        .IgnoreThen(Token.EqualTo(SqlToken.Not))
        .IgnoreThen(Token.EqualTo(SqlToken.Exists))
        .Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, string> Identifier =
        Token.EqualTo(SqlToken.Identifier).Apply(input =>
            Result.Value(input.ToString(), input, input.Skip(input.Length)));

    internal static readonly TokenListParser<SqlToken, bool> HasDot =
        Token.EqualTo(SqlToken.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, int> SignedNumber =
        Token.EqualTo(SqlToken.SignedNumber).Apply(Numerics.IntegerInt32);

    internal static readonly TokenListParser<SqlToken, TableName> TableName =
        Identifier
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier, firstIdentifier)
            : new TableName(firstIdentifier, null))));

    internal static readonly TokenListParser<SqlToken, int[]> TypeModifier =
        LParen.Optional()
        .IgnoreThen(SignedNumber.ManyDelimitedBy(Comma)
        .Then(numbers => RParen.Value(numbers)))
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<SqlToken, TypeName> TypeName =
        Identifier
        .Then(name => TypeModifier
        .Select(signedNumbers => new TypeName(name, signedNumbers)));

    //internal static readonly TokenListParser<SqlToken, ColumnDef> Column =
    //    Token.EqualTo(SqlToken.Identifier)
    //    .Then(name => Token.EqualTo(SqlToken.ColumnType)
    //    .Select(type => new ColumnDef(name.Span.ToString(), type.Span.ToString(), false, false)));

    //internal static TextParser<ColumnDef> Column { get; } =
    //    Span.WhiteSpace.Optional().IgnoreThen(Identifier.Then(name =>
    //        Span.WhiteSpace.IgnoreThen(ColumnType.Then(type =>
    //            Span.WhiteSpace.Optional().Value(new ColumnDef(name, type, false, false))))));

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
