using Squeal.CreateStatement;
using Squeal.CreateStatement.ColumnConstraints;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Squeal;

public static class Ddl
{
    public static readonly Tokenizer<SqlToken> Tokenizer = new TokenizerBuilder<SqlToken>()
        .Ignore(Span.WhiteSpace)
        .Match(Character.EqualTo('('), SqlToken.LParen)
        .Match(Character.EqualTo(')'), SqlToken.RParen)
        .Match(Character.EqualTo(','), SqlToken.Comma)
        .Match(Character.EqualTo('.'), SqlToken.Dot)
        .Match(Numerics.Integer, SqlToken.SignedNumber)
        .Match(Span.EqualToIgnoreCase("ABORT"), SqlToken.Abort, true)
        .Match(Span.EqualToIgnoreCase("ACTION"), SqlToken.Action, true)
        .Match(Span.EqualToIgnoreCase("ALWAYS"), SqlToken.Always, true)
        .Match(Span.EqualToIgnoreCase("AS"), SqlToken.As, true)
        .Match(Span.EqualToIgnoreCase("ASC"), SqlToken.Asc, true)
        .Match(Span.EqualToIgnoreCase("AUTOINCREMENT"), SqlToken.Autoincrement, true)
        .Match(Span.EqualToIgnoreCase("BETWEEN"), SqlToken.Between, true)
        .Match(Span.EqualToIgnoreCase("BLOB"), SqlToken.ColumnTypeBlob, true)
        .Match(Span.EqualToIgnoreCase("CASCADE"), SqlToken.Cascade, true)
        .Match(Span.EqualToIgnoreCase("CASE"), SqlToken.Case, true)
        .Match(Span.EqualToIgnoreCase("CAST"), SqlToken.Cast, true)
        .Match(Span.EqualToIgnoreCase("CHECK"), SqlToken.Check, true)
        .Match(Span.EqualToIgnoreCase("COLLATE"), SqlToken.Collate, true)
        .Match(Span.EqualToIgnoreCase("CONFLICT"), SqlToken.Conflict, true)
        .Match(Span.EqualToIgnoreCase("CONSTRAINT"), SqlToken.Constraint, true)
        .Match(Span.EqualToIgnoreCase("CREATE"), SqlToken.Create, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_DATE"), SqlToken.CurrentDate, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_TIME"), SqlToken.CurrentTime, true)
        .Match(Span.EqualToIgnoreCase("CURRENT_TIMESTAMP"), SqlToken.CurrentTimestamp, true)
        .Match(Span.EqualToIgnoreCase("DEFAULT"), SqlToken.Default, true)
        .Match(Span.EqualToIgnoreCase("DEFERRABLE"), SqlToken.Deferrable, true)
        .Match(Span.EqualToIgnoreCase("DEFERRED"), SqlToken.Deferred, true)
        .Match(Span.EqualToIgnoreCase("DELETE"), SqlToken.Delete, true)
        .Match(Span.EqualToIgnoreCase("DESC"), SqlToken.Desc, true)
        .Match(Span.EqualToIgnoreCase("DISTINCT"), SqlToken.Distinct, true)
        .Match(Span.EqualToIgnoreCase("ELSE"), SqlToken.Else, true)
        .Match(Span.EqualToIgnoreCase("END"), SqlToken.End, true)
        .Match(Span.EqualToIgnoreCase("EXISTS"), SqlToken.Exists, true)
        .Match(Span.EqualToIgnoreCase("FAIL"), SqlToken.Fail, true)
        .Match(Span.EqualToIgnoreCase("FALSE"), SqlToken.False)
        .Match(Span.EqualToIgnoreCase("FOREIGN"), SqlToken.Foreign, true)
        .Match(Span.EqualToIgnoreCase("FROM"), SqlToken.From, true)
        .Match(Span.EqualToIgnoreCase("GENERATED"), SqlToken.Generated, true)
        .Match(Span.EqualToIgnoreCase("IF"), SqlToken.If, true)
        .Match(Span.EqualToIgnoreCase("IGNORE"), SqlToken.Ignore, true)
        .Match(Span.EqualToIgnoreCase("IMMEDIATE"), SqlToken.Immediate, true)
        .Match(Span.EqualToIgnoreCase("IN"), SqlToken.In, true)
        .Match(Span.EqualToIgnoreCase("INDEX"), SqlToken.Index, true)
        .Match(Span.EqualToIgnoreCase("INITIALLY"), SqlToken.Initially, true)
        .Match(Span.EqualToIgnoreCase("INTEGER").Try().Or(Span.EqualToIgnoreCase("INT")), SqlToken.ColumnTypeInteger, true)
        .Match(Span.EqualToIgnoreCase("ISNULL"), SqlToken.IsNull, true)
        .Match(Span.EqualToIgnoreCase("KEY"), SqlToken.Key, true)
        .Match(Span.EqualToIgnoreCase("LIKE"), SqlToken.Like, true)
        .Match(Span.EqualToIgnoreCase("MATCH"), SqlToken.Match, true)
        .Match(Span.EqualToIgnoreCase("NO"), SqlToken.No, true)
        .Match(Span.EqualToIgnoreCase("NOT"), SqlToken.Not, true)
        .Match(Span.EqualToIgnoreCase("NOTNULL"), SqlToken.NotNull, true)
        .Match(Span.EqualToIgnoreCase("NULL"), SqlToken.Null, true)
        .Match(Span.EqualToIgnoreCase("NUMERIC").Try().Or(Span.EqualToIgnoreCase("NUM")), SqlToken.ColumnTypeNumeric, true)
        .Match(Span.EqualToIgnoreCase("ON"), SqlToken.On, true)
        .Match(Span.EqualToIgnoreCase("PRIMARY"), SqlToken.Primary, true)
        .Match(Span.EqualToIgnoreCase("REAL"), SqlToken.ColumnTypeReal, true)
        .Match(Span.EqualToIgnoreCase("REFERENCES"), SqlToken.References, true)
        .Match(Span.EqualToIgnoreCase("REPLACE"), SqlToken.Replace, true)
        .Match(Span.EqualToIgnoreCase("RESTRICT"), SqlToken.Restrict, true)
        .Match(Span.EqualToIgnoreCase("ROLLBACK"), SqlToken.Rollback, true)
        .Match(Span.EqualToIgnoreCase("SET"), SqlToken.Set, true)
        .Match(Span.EqualToIgnoreCase("STORED"), SqlToken.Stored, true)
        .Match(Span.EqualToIgnoreCase("TABLE"), SqlToken.Table, true)
        .Match(Span.EqualToIgnoreCase("TEMPORARY").Try().Or(Span.EqualToIgnoreCase("TEMP")), SqlToken.IsTemporary, true)
        .Match(Span.EqualToIgnoreCase("TEXT"), SqlToken.ColumnTypeText, true)
        .Match(Span.EqualToIgnoreCase("THEN"), SqlToken.Then, true)
        .Match(Span.EqualToIgnoreCase("TRIGGER"), SqlToken.Trigger, true)
        .Match(Span.EqualToIgnoreCase("TRUE"), SqlToken.True)
        .Match(Span.EqualToIgnoreCase("UNIQUE"), SqlToken.Unique, true)
        .Match(Span.EqualToIgnoreCase("VIEW"), SqlToken.View, true)
        .Match(Span.EqualToIgnoreCase("VIRTUAL"), SqlToken.Virtual, true)
        .Match(Span.EqualToIgnoreCase("WHEN"), SqlToken.When, true)
        .Match(Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*"), SqlToken.Identifier, true)
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

    internal static readonly TokenListParser<SqlToken, bool> IsTemporary =
        Token.EqualTo(SqlToken.IsTemporary).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, bool> HasDot =
        Token.EqualTo(SqlToken.Dot).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, int> SignedNumber =
        Token.EqualTo(SqlToken.SignedNumber).Apply(Numerics.IntegerInt32);

    internal static readonly TokenListParser<SqlToken, bool> IfNotExists =
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

    internal static readonly TokenListParser<SqlToken, ColumnTypes> ColumnType =
        Token.EqualTo(SqlToken.ColumnTypeInteger).Value(ColumnTypes.INTEGER)
        .Or(Token.EqualTo(SqlToken.ColumnTypeText).Value(ColumnTypes.TEXT))
        .Or(Token.EqualTo(SqlToken.ColumnTypeNumeric).Value(ColumnTypes.NUMERIC))
        .Or(Token.EqualTo(SqlToken.ColumnTypeReal).Value(ColumnTypes.REAL))
        .Or(Token.EqualTo(SqlToken.ColumnTypeBlob).Value(ColumnTypes.BLOB))
        .OptionalOrDefault(ColumnTypes.BLOB);

    internal static readonly TokenListParser<SqlToken, int[]> ColumnTypeModifier =
        LParen
        .IgnoreThen(SignedNumber.ManyDelimitedBy(Comma, RParen)
        .Select(numbers => numbers))
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<SqlToken, TypeName> ColumnTypeName =
        ColumnType
        .Then(type => ColumnTypeModifier
        .Select(modifiers => new TypeName(type, modifiers)));

    internal static readonly TokenListParser<SqlToken, string> ColumnConstraintName =
        Token.EqualTo(SqlToken.Constraint)
        .IgnoreThen(Identifier.Apply(AsString))
        .OptionalOrDefault(String.Empty);

    internal static readonly TokenListParser<SqlToken, ConflictResolutions> ConcflictClause =
        Token.EqualTo(SqlToken.On)
        .IgnoreThen(Token.EqualTo(SqlToken.Conflict))
        .IgnoreThen(
            Token.EqualTo(SqlToken.Rollback).Value(ConflictResolutions.Rollback)
            .Or(Token.EqualTo(SqlToken.Abort).Value(ConflictResolutions.Abort))
            .Or(Token.EqualTo(SqlToken.Fail).Value(ConflictResolutions.Fail))
            .Or(Token.EqualTo(SqlToken.Ignore).Value(ConflictResolutions.Ignore))
            .Or(Token.EqualTo(SqlToken.Replace).Value(ConflictResolutions.Replace)))
        .OptionalOrDefault(ConflictResolutions.Default);

    internal static readonly TokenListParser<SqlToken, bool> Autoincrement =
        Token.EqualTo(SqlToken.Autoincrement)
        .Value(true)
        .OptionalOrDefault(false);

    internal static TokenListParser<SqlToken, IColumnConstraint> PrimaryKey(string constraintName) =>
        Token.EqualTo(SqlToken.Primary)
        .IgnoreThen(Token.EqualTo(SqlToken.Key))
        .IgnoreThen(Token.EqualTo(SqlToken.Asc).Value(Order.Asc)
            .Or(Token.EqualTo(SqlToken.Desc).Value(Order.Desc))
            .OptionalOrDefault(Order.Asc))
        .Then(order => ConcflictClause
        .Then(resolution => Autoincrement
        .Select(autoIncrement => (IColumnConstraint)new PrimaryKeyConstraint(
            constraintName,
            order,
            resolution,
            autoIncrement))));

    internal static TokenListParser<SqlToken, IColumnConstraint> NotNull(string constraintName) =>
        Token.EqualTo(SqlToken.Not)
        .IgnoreThen(Token.EqualTo(SqlToken.Null))
        .IgnoreThen(ConcflictClause)
        .Select(resolution => (IColumnConstraint)new NotNullConstraint(constraintName, resolution));

    internal static TokenListParser<SqlToken, IColumnConstraint> Unique(string constraintName) =>
        Token.EqualTo(SqlToken.Unique)
        .IgnoreThen(ConcflictClause)
        .Select(resolution => (IColumnConstraint)new UniqueConstraint(constraintName, resolution));

    internal static TokenListParser<SqlToken, IColumnConstraint> Collate(string constraintName) =>
        Token.EqualTo(SqlToken.Collate)
        .IgnoreThen(Identifier.Apply(AsString))
        .Select(identifier => (IColumnConstraint)new CollateConstraint(constraintName, identifier));

    internal static readonly TokenListParser<SqlToken, IColumnConstraint> ColumnConstraint =
        ColumnConstraintName.Then(name =>
            PrimaryKey(name)
            .Or(NotNull(name))
            .Or(Unique(name))
            .Or(Collate(name))
            .Select(cc => cc));

    internal static readonly TokenListParser<SqlToken, ColumnDef> Column =
        Identifier.Apply(AsString)
        .Then(name => ColumnTypeName
        .Then(typeName => ColumnConstraint.Many()
        .Select(constraints => new ColumnDef(name, typeName, constraints))));

    internal static TokenListParser<SqlToken, ColumnDef[]> Columns =
        LParen
        .IgnoreThen(Column.ManyDelimitedBy(Comma, RParen))
        .Select(columns => columns)
        .OptionalOrDefault([]);

    public static readonly TokenListParser<SqlToken, CreateTableStatement> CreateTableStatement =
        Token.EqualTo(SqlToken.Create)
        .IgnoreThen(IsTemporary)
        .Then(isTemp => Token.EqualTo(SqlToken.Table)
        .IgnoreThen(IfNotExists)
        .Then(ifNotExists => TableName
        .Then(name => Columns
        .Select(columns => new CreateTableStatement(name, isTemp, ifNotExists, columns)))));
}
