using Squeal.Create;
using Squeal.Create.ColumnConstraints;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using System.Text.RegularExpressions;

namespace Squeal;

public static class Ddl
{
    private const RegexOptions PatternOptions =
        RegexOptions.ExplicitCapture |
        RegexOptions.Compiled |
        RegexOptions.Singleline |
        RegexOptions.CultureInvariant;

    public static CreateTableStatement Parse(string ddl) =>
        CreateTableStatement.Parse(Tokenizer.Tokenize(ddl));

    public static TokenListParserResult<DdlTokens, CreateTableStatement> TryParse(string ddl) =>
        CreateTableStatement.TryParse(Tokenizer.Tokenize(ddl));

    public enum DdlTokens
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

    internal static readonly Tokenizer<DdlTokens> Tokenizer = new TokenizerBuilder<DdlTokens>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('('), DdlTokens.LParen)
        .Match(Character.EqualTo(')'), DdlTokens.RParen)
        .Match(Character.EqualTo(','), DdlTokens.Comma)
        .Match(Character.EqualTo('.'), DdlTokens.Dot)
        .Match(Numerics.Integer, DdlTokens.SignedNumber)
        .Match(Span.EqualToIgnoreCase("ABORT"), DdlTokens.Abort, true)
        .Match(Span.EqualToIgnoreCase("ACTION"), DdlTokens.Action, true)
        .Match(Span.EqualToIgnoreCase("ALWAYS"), DdlTokens.Always, true)
        .Match(Span.EqualToIgnoreCase("AS"), DdlTokens.As, true)
        .Match(Span.EqualToIgnoreCase("ASC"), DdlTokens.Asc, true)
        .Match(Span.EqualToIgnoreCase("AUTOINCREMENT"), DdlTokens.Autoincrement, true)
        .Match(Span.EqualToIgnoreCase("BETWEEN"), DdlTokens.Between, true)
        .Match(Span.EqualToIgnoreCase("BLOB"), DdlTokens.ColumnTypeBlob, true)
        .Match(Span.EqualToIgnoreCase("CASCADE"), DdlTokens.Cascade, true)
        .Match(Span.EqualToIgnoreCase("CASE"), DdlTokens.Case, true)
        .Match(Span.EqualToIgnoreCase("CAST"), DdlTokens.Cast, true)
        .Match(Span.EqualToIgnoreCase("CHECK"), DdlTokens.Check, true)
        .Match(Span.EqualToIgnoreCase("COLLATE"), DdlTokens.Collate, true)
        .Match(Span.EqualToIgnoreCase("CONFLICT"), DdlTokens.Conflict, true)
        .Match(Span.EqualToIgnoreCase("CONSTRAINT"), DdlTokens.Constraint, true)
        .Match(Span.EqualToIgnoreCase("CREATE"), DdlTokens.Create, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_DATE"), DdlTokens.CurrentDate, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_TIME"), DdlTokens.CurrentTime, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_TIMESTAMP"), DdlTokens.CurrentTimestamp, true)
        .Match(Span.EqualToIgnoreCase("DEFAULT"), DdlTokens.Default, true)
        .Match(Span.EqualToIgnoreCase("DEFERRABLE"), DdlTokens.Deferrable, true)
        .Match(Span.EqualToIgnoreCase("DEFERRED"), DdlTokens.Deferred, true)
        .Match(Span.EqualToIgnoreCase("DELETE"), DdlTokens.Delete, true)
        .Match(Span.EqualToIgnoreCase("DESC"), DdlTokens.Desc, true)
        .Match(Span.EqualToIgnoreCase("DISTINCT"), DdlTokens.Distinct, true)
        .Match(Span.EqualToIgnoreCase("ELSE"), DdlTokens.Else, true)
        .Match(Span.EqualToIgnoreCase("END"), DdlTokens.End, true)
        .Match(Span.EqualToIgnoreCase("EXISTS"), DdlTokens.Exists, true)
        .Match(Span.EqualToIgnoreCase("FAIL"), DdlTokens.Fail, true)
        .Match(Span.EqualToIgnoreCase("FALSE"), DdlTokens.False)
        .Match(Span.EqualToIgnoreCase("FOREIGN"), DdlTokens.Foreign, true)
        .Match(Span.EqualToIgnoreCase("FROM"), DdlTokens.From, true)
        .Match(Span.EqualToIgnoreCase("GENERATED"), DdlTokens.Generated, true)
        .Match(Span.EqualToIgnoreCase("IF"), DdlTokens.If, true)
        .Match(Span.EqualToIgnoreCase("IGNORE"), DdlTokens.Ignore, true)
        .Match(Span.EqualToIgnoreCase("IMMEDIATE"), DdlTokens.Immediate, true)
        .Match(Span.EqualToIgnoreCase("IN"), DdlTokens.In, true)
        .Match(Span.EqualToIgnoreCase("INDEX"), DdlTokens.Index, true)
        .Match(Span.EqualToIgnoreCase("INITIALLY"), DdlTokens.Initially, true)
        .Match(Span.EqualToIgnoreCase("INTEGER").Try().Or(Span.EqualToIgnoreCase("INT")), DdlTokens.ColumnTypeInteger, true)
        .Match(Span.EqualToIgnoreCase("ISNULL"), DdlTokens.IsNull, true)
        .Match(Span.EqualToIgnoreCase("KEY"), DdlTokens.Key, true)
        .Match(Span.EqualToIgnoreCase("LIKE"), DdlTokens.Like, true)
        .Match(Span.EqualToIgnoreCase("MATCH"), DdlTokens.Match, true)
        .Match(Span.EqualToIgnoreCase("NO"), DdlTokens.No, true)
        .Match(Span.EqualToIgnoreCase("NOT"), DdlTokens.Not, true)
        .Match(Span.EqualToIgnoreCase("NOTNULL"), DdlTokens.NotNull, true)
        .Match(Span.EqualToIgnoreCase("NULL"), DdlTokens.Null, true)
        .Match(Span.EqualToIgnoreCase("NUMERIC").Try().Or(Span.EqualToIgnoreCase("NUM")), DdlTokens.ColumnTypeNumeric, true)
        .Match(Span.EqualToIgnoreCase("ON"), DdlTokens.On, true)
        .Match(Span.EqualToIgnoreCase("PRIMARY"), DdlTokens.Primary, true)
        .Match(Span.EqualToIgnoreCase("REAL"), DdlTokens.ColumnTypeReal, true)
        .Match(Span.EqualToIgnoreCase("REFERENCES"), DdlTokens.References, true)
        .Match(Span.EqualToIgnoreCase("REPLACE"), DdlTokens.Replace, true)
        .Match(Span.EqualToIgnoreCase("RESTRICT"), DdlTokens.Restrict, true)
        .Match(Span.EqualToIgnoreCase("ROLLBACK"), DdlTokens.Rollback, true)
        .Match(Span.EqualToIgnoreCase("SET"), DdlTokens.Set, true)
        .Match(Span.EqualToIgnoreCase("STORED"), DdlTokens.Stored, true)
        .Match(Span.EqualToIgnoreCase("TABLE"), DdlTokens.Table, true)
        .Match(Span.EqualToIgnoreCase("TEMPORARY").Try().Or(Span.EqualToIgnoreCase("TEMP")), DdlTokens.IsTemporary, true)
        .Match(Span.EqualToIgnoreCase("TEXT"), DdlTokens.ColumnTypeText, true)
        .Match(Span.EqualToIgnoreCase("THEN"), DdlTokens.Then, true)
        .Match(Span.EqualToIgnoreCase("TRIGGER"), DdlTokens.Trigger, true)
        .Match(Span.EqualToIgnoreCase("TRUE"), DdlTokens.True)
        .Match(Span.EqualToIgnoreCase("UNIQUE"), DdlTokens.Unique, true)
        .Match(Span.EqualToIgnoreCase("VIEW"), DdlTokens.View, true)
        .Match(Span.EqualToIgnoreCase("VIRTUAL"), DdlTokens.Virtual, true)
        .Match(Span.EqualToIgnoreCase("WHEN"), DdlTokens.When, true)
        .Match(Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*", PatternOptions), DdlTokens.Identifier, true)
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

    internal static readonly TokenListParser<DdlTokens, Token<DdlTokens>> Comma =
        Token.EqualTo(DdlTokens.Comma);

    internal static readonly TokenListParser<DdlTokens, Token<DdlTokens>> LParen =
        Token.EqualTo(DdlTokens.LParen);

    internal static readonly TokenListParser<DdlTokens, Token<DdlTokens>> RParen =
        Token.EqualTo(DdlTokens.RParen);

    internal static readonly TokenListParser<DdlTokens, Token<DdlTokens>> Identifier =
        Token.EqualTo(DdlTokens.Identifier);

    internal static readonly TokenListParser<DdlTokens, bool> IsTemporary =
        Token.EqualTo(DdlTokens.IsTemporary).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<DdlTokens, bool> HasDot =
        Token.EqualTo(DdlTokens.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<DdlTokens, int> SignedNumber =
        Token.EqualTo(DdlTokens.SignedNumber).Apply(Numerics.IntegerInt32);

    internal static readonly TokenListParser<DdlTokens, bool> IfNotExists =
        Token.EqualTo(DdlTokens.If)
        .IgnoreThen(Token.EqualTo(DdlTokens.Not))
        .IgnoreThen(Token.EqualTo(DdlTokens.Exists))
        .Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<DdlTokens, TableName> TableName =
        Identifier.Apply(Squeal.Parse.AsString)
        .Then(firstIdentifier => HasDot
        .Then(hasDot => Identifier.Apply(Squeal.Parse.AsString).OptionalOrDefault(String.Empty)
        .Select(secondIdentifier => hasDot
            ? new TableName(secondIdentifier, firstIdentifier)
            : new TableName(firstIdentifier, null))));

    internal static readonly TokenListParser<DdlTokens, ColumnTypes> ColumnType =
        Token.EqualTo(DdlTokens.ColumnTypeInteger).Value(ColumnTypes.INTEGER)
        .Or(Token.EqualTo(DdlTokens.ColumnTypeText).Value(ColumnTypes.TEXT))
        .Or(Token.EqualTo(DdlTokens.ColumnTypeNumeric).Value(ColumnTypes.NUMERIC))
        .Or(Token.EqualTo(DdlTokens.ColumnTypeReal).Value(ColumnTypes.REAL))
        .Or(Token.EqualTo(DdlTokens.ColumnTypeBlob).Value(ColumnTypes.BLOB))
        .OptionalOrDefault(ColumnTypes.BLOB);

    internal static readonly TokenListParser<DdlTokens, int[]> ColumnTypeModifier =
        LParen
        .IgnoreThen(SignedNumber.ManyDelimitedBy(Comma, RParen)
        .Select(numbers => numbers))
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<DdlTokens, TypeName> ColumnTypeName =
        ColumnType
        .Then(type => ColumnTypeModifier
        .Select(modifiers => new TypeName(type, modifiers)));

    internal static readonly TokenListParser<DdlTokens, string> ColumnConstraintName =
        Token.EqualTo(DdlTokens.Constraint)
        .IgnoreThen(Identifier.Apply(Squeal.Parse.AsString))
        .OptionalOrDefault(String.Empty);

    internal static readonly TokenListParser<DdlTokens, ConflictResolutions> ConcflictClause =
        Token.EqualTo(DdlTokens.On)
        .IgnoreThen(Token.EqualTo(DdlTokens.Conflict))
        .IgnoreThen(
            Token.EqualTo(DdlTokens.Rollback).Value(ConflictResolutions.Rollback)
            .Or(Token.EqualTo(DdlTokens.Abort).Value(ConflictResolutions.Abort))
            .Or(Token.EqualTo(DdlTokens.Fail).Value(ConflictResolutions.Fail))
            .Or(Token.EqualTo(DdlTokens.Ignore).Value(ConflictResolutions.Ignore))
            .Or(Token.EqualTo(DdlTokens.Replace).Value(ConflictResolutions.Replace)))
        .OptionalOrDefault(ConflictResolutions.Default);

    internal static readonly TokenListParser<DdlTokens, bool> Autoincrement =
        Token.EqualTo(DdlTokens.Autoincrement)
        .Value(true)
        .OptionalOrDefault(false);

    internal static TokenListParser<DdlTokens, IColumnConstraint> PrimaryKey(string constraintName) =>
        Token.EqualTo(DdlTokens.Primary)
        .IgnoreThen(Token.EqualTo(DdlTokens.Key))
        .IgnoreThen(Token.EqualTo(DdlTokens.Asc).Value(Order.Asc)
            .Or(Token.EqualTo(DdlTokens.Desc).Value(Order.Desc))
            .OptionalOrDefault(Order.Asc))
        .Then(order => ConcflictClause
        .Then(resolution => Autoincrement
        .Select(autoIncrement => (IColumnConstraint)new PrimaryKeyConstraint(
            constraintName,
            order,
            resolution,
            autoIncrement))));

    internal static TokenListParser<DdlTokens, IColumnConstraint> NotNull(string constraintName) =>
        Token.EqualTo(DdlTokens.Not)
        .IgnoreThen(Token.EqualTo(DdlTokens.Null))
        .IgnoreThen(ConcflictClause)
        .Select(resolution => (IColumnConstraint)new NotNullConstraint(constraintName, resolution));

    internal static TokenListParser<DdlTokens, IColumnConstraint> Unique(string constraintName) =>
        Token.EqualTo(DdlTokens.Unique)
        .IgnoreThen(ConcflictClause)
        .Select(resolution => (IColumnConstraint)new UniqueConstraint(constraintName, resolution));

    internal static TokenListParser<DdlTokens, IColumnConstraint> Collate(string constraintName) =>
        Token.EqualTo(DdlTokens.Collate)
        .IgnoreThen(Identifier.Apply(Squeal.Parse.AsString))
        .Select(identifier => (IColumnConstraint)new CollateConstraint(constraintName, identifier));

    internal static readonly TokenListParser<DdlTokens, IColumnConstraint> ColumnConstraint =
        ColumnConstraintName.Then(name =>
            PrimaryKey(name)
            .Or(NotNull(name))
            .Or(Unique(name))
            .Or(Collate(name))
            .Select(cc => cc));

    internal static readonly TokenListParser<DdlTokens, ColumnDef> Column =
        Identifier.Apply(Squeal.Parse.AsString)
        .Then(name => ColumnTypeName
        .Then(typeName => ColumnConstraint.Many()
        .Select(constraints => new ColumnDef(name, typeName, constraints))));

    internal static TokenListParser<DdlTokens, ColumnDef[]> Columns =
        LParen
        .IgnoreThen(Column.ManyDelimitedBy(Comma, RParen))
        .Select(columns => columns)
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<DdlTokens, CreateTableStatement> CreateTableStatement =
        Token.EqualTo(DdlTokens.Create)
        .IgnoreThen(IsTemporary)
        .Then(isTemp => Token.EqualTo(DdlTokens.Table)
        .IgnoreThen(IfNotExists)
        .Then(ifNotExists => TableName
        .Then(name => Columns
        .Select(columns => new CreateTableStatement(name, isTemp, ifNotExists, columns)))));
}
