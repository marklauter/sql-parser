namespace Squeal.Select.Expressions;

public enum ConditionalOperators
{
    EqualTo = Sql.SelectTokens.EqualTo,
    NotEqualTo = Sql.SelectTokens.NotEqualTo,
    GreaterThan = Sql.SelectTokens.GreaterThan,
    GreaterThanOrEqualTo = Sql.SelectTokens.GreaterThanOrEqualTo,
    LessThan = Sql.SelectTokens.LessThan,
    LessThanOrEqualTo = Sql.SelectTokens.LessThanOrEqualTo,
}
