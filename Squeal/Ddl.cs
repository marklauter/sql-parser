﻿using Squeal.CreateStatement;
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

    internal static readonly TokenListParser<SqlToken, int[]> TypeModifier =
        LParen
        .IgnoreThen(SignedNumber.ManyDelimitedBy(Comma, RParen)
        .Select(numbers => numbers))
        .OptionalOrDefault([]);

    internal static readonly TokenListParser<SqlToken, TypeName> TypeName =
        ColumnType
        .Then(type => TypeModifier
        .Select(modifiers => new TypeName(type, modifiers)));

    internal static readonly TokenListParser<SqlToken, string> ColumnConstraintName =
        Token.EqualTo(SqlToken.Constraint)
        .IgnoreThen(Identifier.Apply(AsString))
        .OptionalOrDefault(String.Empty);

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> NotNull =
        Token.EqualTo(SqlToken.Not)
        .IgnoreThen(Token.EqualTo(SqlToken.Null).Value(ColumnConstraints.NotNull));

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> GeneratedAlways =
        Token.EqualTo(SqlToken.Generated)
        .IgnoreThen(Token.EqualTo(SqlToken.Always).Value(ColumnConstraints.Generated));

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> PrimaryKey =
        Token.EqualTo(SqlToken.Primary)
        .IgnoreThen(Token.EqualTo(SqlToken.Key))
        .IgnoreThen(Token.EqualTo(SqlToken.Asc)
            .Value(ColumnConstraints.PrimaryKeyAsc))
        .IgnoreThen(Token.EqualTo(SqlToken.Desc)
            .Value(ColumnConstraints.PrimaryKeyDesc)
            .OptionalOrDefault(ColumnConstraints.PrimaryKeyAsc));

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> Unique =
        Token.EqualTo(SqlToken.Unique).Value(ColumnConstraints.Unique);

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> Check =
        Token.EqualTo(SqlToken.Check).Value(ColumnConstraints.Check);

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> Default =
        Token.EqualTo(SqlToken.Default).Value(ColumnConstraints.Default);

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> Collate =
        Token.EqualTo(SqlToken.Collate).Value(ColumnConstraints.Collate);

    internal static readonly TokenListParser<SqlToken, ColumnConstraints> As =
        Token.EqualTo(SqlToken.As).Value(ColumnConstraints.As);

    // todo: need to add foreign key constraint
    internal static readonly TokenListParser<SqlToken, ColumnConstraintKind> ConstraintKind =
        ColumnConstraintName.Then(constraintName =>
        PrimaryKey
        .Or(NotNull)
        .Or(Unique)
        .Or(Check)
        .Or(Default)
        .Or(Collate)
        .Or(GeneratedAlways)
        .Or(As)
        .Select(type => new ColumnConstraintKind(constraintName, type)))
        .OptionalOrDefault(ColumnConstraintKind.Default);

    internal static readonly TokenListParser<SqlToken, bool> Autoincrement = 
        Token.EqualTo(SqlToken.Autoincrement).Value(true).OptionalOrDefault(false);

    internal static readonly TokenListParser<SqlToken, ConflictResolutions> ConcflictClause = 
        Token.EqualTo(SqlToken.On)
        .IgnoreThen(Token.EqualTo(SqlToken.Conflict))
        .IgnoreThen(
            Token.EqualTo(SqlToken.Rollback).Value(ConflictResolutions.Rollback)
            .Or(Token.EqualTo(SqlToken.Abort).Value(ConflictResolutions.Abort))
            .Or(Token.EqualTo(SqlToken.Fail).Value(ConflictResolutions.Fail))
            .Or(Token.EqualTo(SqlToken.Ignore).Value(ConflictResolutions.Ignore))
            .Or(Token.EqualTo(SqlToken.Replace).Value(ConflictResolutions.Replace)))
        .OptionalOrDefault(ConflictResolutions.Undefined);

    internal static readonly TokenListParser<SqlToken, ColumnDef> Column =
        Identifier.Apply(AsString)
        .Then(name => TypeName
        .Select(type => new ColumnDef(name, type, null)));

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
