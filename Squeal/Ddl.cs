using Squeal.CreateStatement;
using Superpower;
using Superpower.Parsers;

namespace Squeal;

public static class Ddl
{
    public static void Foo()
    {
        var x =
            Token.EqualTo(SqlToken.Identifier)
           .Then(firstIdentifier => Token.EqualTo(SqlToken.Dot).Value(true).OptionalOrDefault(false)
           .Then(hasDot => Token.EqualTo(SqlToken.Identifier)
           .Select(secondIdentifier => hasDot
                ? new TableName(secondIdentifier.Span.ToString(), firstIdentifier.Span.ToString())
                : new TableName(firstIdentifier.Span.ToString(), null))));
    }

    private static readonly TokenListParser<SqlToken, bool> IsTemporary =
        Token.EqualTo(SqlToken.IsTemporary).Value(true).OptionalOrDefault(false);

    private static readonly TokenListParser<SqlToken, bool> IfNotExists =
        Token.EqualTo(SqlToken.If)
        .IgnoreThen(Token.EqualTo(SqlToken.Not))
        .IgnoreThen(Token.EqualTo(SqlToken.Exists))
        .Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, TableName> TableName =
        Token.EqualTo(SqlToken.Identifier)
        .Then(firstIdentifier => Token.EqualTo(SqlToken.Dot).Value(true).OptionalOrDefault(false)
        .Then(hasDot => Token.EqualTo(SqlToken.Identifier).OptionalOrDefault()
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier.Span.ToString(), firstIdentifier.Span.ToString())
            : new TableName(firstIdentifier.Span.ToString(), null))));

    public static readonly TokenListParser<SqlToken, CreateTableStatement> CreateTable =
        Token.EqualTo(SqlToken.Create)
        .IgnoreThen(IsTemporary)
        .Then(isTemp => Token.EqualTo(SqlToken.Table)
        .IgnoreThen(IfNotExists)
        .Then(ifNotExists => TableName
        .Select(name => new CreateTableStatement(name, isTemp, ifNotExists))));

    //internal static TextParser<ColumnDef> Column { get; } =
    //    Span.WhiteSpace.Optional().IgnoreThen(Identifier.Then(name =>
    //        Span.WhiteSpace.IgnoreThen(ColumnType.Then(type =>
    //            Span.WhiteSpace.Optional().Value(new ColumnDef(name, type, false, false))))));

    //internal static TextParser<ColumnDef[]> Columns { get; } =
    //    Span.WhiteSpace.Optional()
    //    .IgnoreThen(OpenParen)
    //    .IgnoreThen(Column.ManyDelimitedBy(Comma))
    //    .Then(columns => CloseParen.Value(columns));
}
