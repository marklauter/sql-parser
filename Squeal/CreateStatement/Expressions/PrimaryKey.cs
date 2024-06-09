using Squeal.Expressions;
using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record PrimaryKey(Order Order, ConflictClause ConflictClause, bool AutoIncrement) : Expression;
