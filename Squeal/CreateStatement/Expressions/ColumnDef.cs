using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record ColumnDef(Identifier Name, ColumnType Type, ColumnConstraint Constraint, ColumnDef? NextColumn) : Expression;
