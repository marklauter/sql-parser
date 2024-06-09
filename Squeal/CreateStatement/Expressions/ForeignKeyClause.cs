using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record ForeignKeyClause(
    Identifier ForeignTable,
    Identifier[] ColumnNames,
    ForeignKeyAction? OnDelete,
    ForeignKeyAction? OnUpdate,
    Identifier? MatchName,
    Deferable? Deferable) : Expression;
