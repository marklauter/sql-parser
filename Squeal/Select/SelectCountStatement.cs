namespace Squeal.Select;

public record SelectCountStatement(TableName From, Predicate Where)
    : ISelectStatement;
