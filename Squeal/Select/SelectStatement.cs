namespace Squeal.Select;

public record SelectStatement(TableName From, ProjectedColumn[] Projection, Predicate Where)
    : ISelectStatement;
