namespace Squeal.CreateStatement;

public sealed record TableConstraint();

public sealed record TableOptions();

public sealed record CreateTableStatement(
    TableName TableName,
    bool IsTemp,
    bool IfNotExists,
    TableConstraint? Constraint = null,
    TableOptions? Options = null);
