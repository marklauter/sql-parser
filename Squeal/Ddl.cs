using Squeal.Create;
using Squeal.Create.ColumnConstraints;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Squeal;

public static class Ddl
{
    public static TokenListParserResult<DdlToken, CreateTableStatement> TryParse(string ddl)
    {
        var tokens = Tokenizer.TryTokenize(ddl);
        return CreateTableStatement.TryParse(tokens.Value);
    }

    public enum DdlToken
    {
        False,
        True,
        LParen,
        RParen,
        Comma,
        Dot,
        Create,
        Trigger,
        View,
        Index,
        Table,
        IsTemporary,
        Identifier,
        TableName,
        Exists,
        Not,
        If,
        As,
        SignedNumber,
        Constraint,
        Primary,
        Key,
        Unique,
        Check,
        Foreign,
        Null,
        Default,
        Collate,
        Generated,
        Always,
        Stored,
        Virtual,
        Asc,
        Desc,
        CurrentTime,
        CurrentDate,
        CurrentTimestamp,
        Autoincrement,
        On,
        Conflict,
        Rollback,
        Abort,
        Fail,
        Ignore,
        Replace,
        From,
        Distinct,
        Between,
        References,
        Set,
        Cascade,
        Delete,
        No,
        Action,
        Match,
        Deferrable,
        Initially,
        Deferred,
        Immediate,
        Restrict,
        Cast,
        Like,
        IsNull,
        NotNull,
        In,
        Case,
        When,
        Then,
        Else,
        End,
        ColumnTypeText,
        ColumnTypeNumeric,
        ColumnTypeInteger,
        ColumnTypeReal,
        ColumnTypeBlob,
    }

    internal static readonly Tokenizer<DdlToken> Tokenizer = new TokenizerBuilder<DdlToken>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('('), DdlToken.LParen)
        .Match(Character.EqualTo(')'), DdlToken.RParen)
        .Match(Character.EqualTo(','), DdlToken.Comma)
        .Match(Character.EqualTo('.'), DdlToken.Dot)
        .Match(Numerics.Integer, DdlToken.SignedNumber)
        .Match(Span.EqualToIgnoreCase("ABORT"), DdlToken.Abort, true)
        .Match(Span.EqualToIgnoreCase("ACTION"), DdlToken.Action, true)
        .Match(Span.EqualToIgnoreCase("ALWAYS"), DdlToken.Always, true)
        .Match(Span.EqualToIgnoreCase("AS"), DdlToken.As, true)
        .Match(Span.EqualToIgnoreCase("ASC"), DdlToken.Asc, true)
        .Match(Span.EqualToIgnoreCase("AUTOINCREMENT"), DdlToken.Autoincrement, true)
        .Match(Span.EqualToIgnoreCase("BETWEEN"), DdlToken.Between, true)
        .Match(Span.EqualToIgnoreCase("BLOB"), DdlToken.ColumnTypeBlob, true)
        .Match(Span.EqualToIgnoreCase("CASCADE"), DdlToken.Cascade, true)
        .Match(Span.EqualToIgnoreCase("CASE"), DdlToken.Case, true)
        .Match(Span.EqualToIgnoreCase("CAST"), DdlToken.Cast, true)
        .Match(Span.EqualToIgnoreCase("CHECK"), DdlToken.Check, true)
        .Match(Span.EqualToIgnoreCase("COLLATE"), DdlToken.Collate, true)
        .Match(Span.EqualToIgnoreCase("CONFLICT"), DdlToken.Conflict, true)
        .Match(Span.EqualToIgnoreCase("CONSTRAINT"), DdlToken.Constraint, true)
        .Match(Span.EqualToIgnoreCase("CREATE"), DdlToken.Create, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_DATE"), DdlToken.CurrentDate, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_TIME"), DdlToken.CurrentTime, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_TIMESTAMP"), DdlToken.CurrentTimestamp, true)
        .Match(Span.EqualToIgnoreCase("DEFAULT"), DdlToken.Default, true)
        .Match(Span.EqualToIgnoreCase("DEFERRABLE"), DdlToken.Deferrable, true)
        .Match(Span.EqualToIgnoreCase("DEFERRED"), DdlToken.Deferred, true)
        .Match(Span.EqualToIgnoreCase("DELETE"), DdlToken.Delete, true)
        .Match(Span.EqualToIgnoreCase("DESC"), DdlToken.Desc, true)
        .Match(Span.EqualToIgnoreCase("DISTINCT"), DdlToken.Distinct, true)
        .Match(Span.EqualToIgnoreCase("ELSE"), DdlToken.Else, true)
        .Match(Span.EqualToIgnoreCase("END"), DdlToken.End, true)
        .Match(Span.EqualToIgnoreCase("EXISTS"), DdlToken.Exists, true)
        .Match(Span.EqualToIgnoreCase("FAIL"), DdlToken.Fail, true)
        .Match(Span.EqualToIgnoreCase("FALSE"), DdlToken.False)
        .Match(Span.EqualToIgnoreCase("FOREIGN"), DdlToken.Foreign, true)
        .Match(Span.EqualToIgnoreCase("FROM"), DdlToken.From, true)
        .Match(Span.EqualToIgnoreCase("GENERATED"), DdlToken.Generated, true)
        .Match(Span.EqualToIgnoreCase("IF"), DdlToken.If, true)
        .Match(Span.EqualToIgnoreCase("IGNORE"), DdlToken.Ignore, true)
        .Match(Span.EqualToIgnoreCase("IMMEDIATE"), DdlToken.Immediate, true)
        .Match(Span.EqualToIgnoreCase("IN"), DdlToken.In, true)
        .Match(Span.EqualToIgnoreCase("INDEX"), DdlToken.Index, true)
        .Match(Span.EqualToIgnoreCase("INITIALLY"), DdlToken.Initially, true)
        .Match(Span.EqualToIgnoreCase("INTEGER").Try().Or(Span.EqualToIgnoreCase("INT")), DdlToken.ColumnTypeInteger, true)
        .Match(Span.EqualToIgnoreCase("ISNULL"), DdlToken.IsNull, true)
        .Match(Span.EqualToIgnoreCase("KEY"), DdlToken.Key, true)
        .Match(Span.EqualToIgnoreCase("LIKE"), DdlToken.Like, true)
        .Match(Span.EqualToIgnoreCase("MATCH"), DdlToken.Match, true)
        .Match(Span.EqualToIgnoreCase("NO"), DdlToken.No, true)
        .Match(Span.EqualToIgnoreCase("NOT"), DdlToken.Not, true)
        .Match(Span.EqualToIgnoreCase("NOTNULL"), DdlToken.NotNull, true)
        .Match(Span.EqualToIgnoreCase("NULL"), DdlToken.Null, true)
        .Match(Span.EqualToIgnoreCase("NUMERIC").Try().Or(Span.EqualToIgnoreCase("NUM")), DdlToken.ColumnTypeNumeric, true)
        .Match(Span.EqualToIgnoreCase("ON"), DdlToken.On, true)
        .Match(Span.EqualToIgnoreCase("PRIMARY"), DdlToken.Primary, true)
        .Match(Span.EqualToIgnoreCase("REAL"), DdlToken.ColumnTypeReal, true)
        .Match(Span.EqualToIgnoreCase("REFERENCES"), DdlToken.References, true)
        .Match(Span.EqualToIgnoreCase("REPLACE"), DdlToken.Replace, true)
        .Match(Span.EqualToIgnoreCase("RESTRICT"), DdlToken.Restrict, true)
        .Match(Span.EqualToIgnoreCase("ROLLBACK"), DdlToken.Rollback, true)
        .Match(Span.EqualToIgnoreCase("SET"), DdlToken.Set, true)
        .Match(Span.EqualToIgnoreCase("STORED"), DdlToken.Stored, true)
        .Match(Span.EqualToIgnoreCase("TABLE"), DdlToken.Table, true)
        .Match(Span.EqualToIgnoreCase("TEMPORARY").Try().Or(Span.EqualToIgnoreCase("TEMP")), DdlToken.IsTemporary, true)
        .Match(Span.EqualToIgnoreCase("TEXT"), DdlToken.ColumnTypeText, true)
        .Match(Span.EqualToIgnoreCase("THEN"), DdlToken.Then, true)
        .Match(Span.EqualToIgnoreCase("TRIGGER"), DdlToken.Trigger, true)
        .Match(Span.EqualToIgnoreCase("TRUE"), DdlToken.True)
        .Match(Span.EqualToIgnoreCase("UNIQUE"), DdlToken.Unique, true)
        .Match(Span.EqualToIgnoreCase("VIEW"), DdlToken.View, true)
        .Match(Span.EqualToIgnoreCase("VIRTUAL"), DdlToken.Virtual, true)
        .Match(Span.EqualToIgnoreCase("WHEN"), DdlToken.When, true)
        .Match(Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), DdlToken.Identifier, true)
        .Build();

    // todo: check, default, generated, as, and foreign key are hard - put them off until later
    //internal static readonly TokenListParser<SqlToken, ColumnConstraintType> Check =
    //    Token.EqualTo(SqlToken.Check).Value(ColumnConstraintType.Check);

    //internal static readonly TokenListParser<SqlToken, ColumnConstraintType> Default =
    //    Token.EqualTo(SqlToken.Default).Value(ColumnConstraintType.Default);

    //internal static readonly TokenListParser<SqlToken, ColumnConstraintType> GeneratedAlways =
    //    Token.EqualTo(SqlToken.Generated)
    //    .IgnoreThen(Token.EqualTo(SqlToken.Always).Value(ColumnConstraintType.Generated));

    //internal static readonly TokenListParser<SqlToken, ColumnConstraintType> As =
    //    Token.EqualTo(SqlToken.As).Value(ColumnConstraintType.As);

    internal static readonly TokenListParser<DdlToken, Token<DdlToken>> Comma =
        Token.EqualTo(DdlToken.Comma);

    internal static readonly TokenListParser<DdlToken, Token<DdlToken>> LParen =
        Token.EqualTo(DdlToken.LParen);

    internal static readonly TokenListParser<DdlToken, Token<DdlToken>> RParen =
        Token.EqualTo(DdlToken.RParen);

    internal static readonly TokenListParser<DdlToken, Token<DdlToken>> Identifier =
        Token.EqualTo(DdlToken.Identifier);

    internal static readonly TokenListParser<DdlToken, bool> IsTemporary =
        Token.EqualTo(DdlToken.IsTemporary).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<DdlToken, bool> HasDot =
        Token.EqualTo(DdlToken.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<DdlToken, int> SignedNumber =
        Token.EqualTo(DdlToken.SignedNumber).Apply(Numerics.IntegerInt32);

    internal static readonly TokenListParser<DdlToken, bool> IfNotExists =
        Token.EqualTo(DdlToken.If)
        .IgnoreThen(Token.EqualTo(DdlToken.Not))
        .IgnoreThen(Token.EqualTo(DdlToken.Exists))
        .Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<DdlToken, TableName> TableName =
        Identifier.Apply(Parse.AsString)
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(Parse.AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier, firstIdentifier)
            : new TableName(firstIdentifier, null))));

    internal static readonly TokenListParser<DdlToken, ColumnTypes> ColumnType =
        Token.EqualTo(DdlToken.ColumnTypeInteger).Value(ColumnTypes.INTEGER)
        .Or(Token.EqualTo(DdlToken.ColumnTypeText).Value(ColumnTypes.TEXT))
        .Or(Token.EqualTo(DdlToken.ColumnTypeNumeric).Value(ColumnTypes.NUMERIC))
        .Or(Token.EqualTo(DdlToken.ColumnTypeReal).Value(ColumnTypes.REAL))
        .Or(Token.EqualTo(DdlToken.ColumnTypeBlob).Value(ColumnTypes.BLOB))
        .OptionalOrDefault(ColumnTypes.BLOB);

    internal static readonly TokenListParser<DdlToken, int[]> ColumnTypeModifier =
        LParen
        .IgnoreThen(SignedNumber.ManyDelimitedBy(Comma, RParen)
        .Select(numbers => numbers))
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<DdlToken, TypeName> ColumnTypeName =
        ColumnType
        .Then(type => ColumnTypeModifier
        .Select(modifiers => new TypeName(type, modifiers)));

    internal static readonly TokenListParser<DdlToken, string> ColumnConstraintName =
        Token.EqualTo(DdlToken.Constraint)
        .IgnoreThen(Identifier.Apply(Parse.AsString))
        .OptionalOrDefault(String.Empty);

    internal static readonly TokenListParser<DdlToken, ConflictResolutions> ConcflictClause =
        Token.EqualTo(DdlToken.On)
        .IgnoreThen(Token.EqualTo(DdlToken.Conflict))
        .IgnoreThen(
            Token.EqualTo(DdlToken.Rollback).Value(ConflictResolutions.Rollback)
            .Or(Token.EqualTo(DdlToken.Abort).Value(ConflictResolutions.Abort))
            .Or(Token.EqualTo(DdlToken.Fail).Value(ConflictResolutions.Fail))
            .Or(Token.EqualTo(DdlToken.Ignore).Value(ConflictResolutions.Ignore))
            .Or(Token.EqualTo(DdlToken.Replace).Value(ConflictResolutions.Replace)))
        .OptionalOrDefault(ConflictResolutions.Default);

    internal static readonly TokenListParser<DdlToken, bool> Autoincrement =
        Token.EqualTo(DdlToken.Autoincrement)
        .Value(true)
        .OptionalOrDefault(false);

    internal static TokenListParser<DdlToken, IColumnConstraint> PrimaryKey(string constraintName) =>
        Token.EqualTo(DdlToken.Primary)
        .IgnoreThen(Token.EqualTo(DdlToken.Key))
        .IgnoreThen(Token.EqualTo(DdlToken.Asc).Value(Order.Asc)
            .Or(Token.EqualTo(DdlToken.Desc).Value(Order.Desc))
            .OptionalOrDefault(Order.Asc))
        .Then(order => ConcflictClause
        .Then(resolution => Autoincrement
        .Select(autoIncrement => (IColumnConstraint)new PrimaryKeyConstraint(
            constraintName,
            order,
            resolution,
            autoIncrement))));

    internal static TokenListParser<DdlToken, IColumnConstraint> NotNull(string constraintName) =>
        Token.EqualTo(DdlToken.Not)
        .IgnoreThen(Token.EqualTo(DdlToken.Null))
        .IgnoreThen(ConcflictClause)
        .Select(resolution => (IColumnConstraint)new NotNullConstraint(constraintName, resolution));

    internal static TokenListParser<DdlToken, IColumnConstraint> Unique(string constraintName) =>
        Token.EqualTo(DdlToken.Unique)
        .IgnoreThen(ConcflictClause)
        .Select(resolution => (IColumnConstraint)new UniqueConstraint(constraintName, resolution));

    internal static TokenListParser<DdlToken, IColumnConstraint> Collate(string constraintName) =>
        Token.EqualTo(DdlToken.Collate)
        .IgnoreThen(Identifier.Apply(Parse.AsString))
        .Select(identifier => (IColumnConstraint)new CollateConstraint(constraintName, identifier));

    internal static readonly TokenListParser<DdlToken, IColumnConstraint> ColumnConstraint =
        ColumnConstraintName.Then(name =>
            PrimaryKey(name)
            .Or(NotNull(name))
            .Or(Unique(name))
            .Or(Collate(name))
            .Select(cc => cc));

    internal static readonly TokenListParser<DdlToken, ColumnDef> Column =
        Identifier.Apply(Parse.AsString)
        .Then(name => ColumnTypeName
        .Then(typeName => ColumnConstraint.Many()
        .Select(constraints => new ColumnDef(name, typeName, constraints))));

    internal static TokenListParser<DdlToken, ColumnDef[]> Columns =
        LParen
        .IgnoreThen(Column.ManyDelimitedBy(Comma, RParen))
        .Select(columns => columns)
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<DdlToken, CreateTableStatement> CreateTableStatement =
        Token.EqualTo(DdlToken.Create)
        .IgnoreThen(IsTemporary)
        .Then(isTemp => Token.EqualTo(DdlToken.Table)
        .IgnoreThen(IfNotExists)
        .Then(ifNotExists => TableName
        .Then(name => Columns
        .Select(columns => new CreateTableStatement(name, isTemp, ifNotExists, columns)))));
}
