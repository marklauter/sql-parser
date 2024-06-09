using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record TableConstraint(
    Identifier? Name,
    PrimaryKey? PrimaryKey,
    Unique? Unique) : Expression;

internal sealed record CreateTableStatement(bool IsTemporary, bool CheckExists, Table? Table, ColumnDef? Columns, TableConstraint? TableConstraint) : Statement;
