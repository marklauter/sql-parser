using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record ColumnConstraint(
    Identifier? Name,
    PrimaryKey? PrimaryKey,
    NotNull? NotNull,
    Unique? Unique,
    Check? Check,
    Expression? Default,
    Collation? Collation,
    ForeignKeyClause? ForeignKeyClause,
    bool GeneratedAlways,
    ParentheticalExpression? As,
    GeneratedKind GeneratedKind) : Expression;
