using Squeal.CreateStatement;
using Squeal.CreateStatement.ColumnConstraints;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Squeal;

public static class Ddl
{
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
