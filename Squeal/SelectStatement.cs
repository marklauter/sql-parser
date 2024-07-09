namespace Squeal;

public record SelectStatement(TableName From, ProjectedColumn[] Projection, Predicate Where)
    : ISelectStatement;
