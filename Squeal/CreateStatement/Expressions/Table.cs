using Squeal.Expressions;

namespace Squeal.CreateStatement.Expressions;

internal sealed record Table(Identifier Name, Identifier? Schema = null);
