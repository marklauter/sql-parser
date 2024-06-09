using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record Check(ParentheticalExpression Expression) : Expression;
