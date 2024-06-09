using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record Unique(ConflictClause ConflictClause) : Expression;
