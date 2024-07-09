namespace Squeal;

public record SelectCountStatement(TableName From, Predicate Where)
    : ISelectStatement;
