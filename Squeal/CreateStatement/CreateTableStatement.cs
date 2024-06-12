namespace Squeal.CreateStatement;

public sealed record CreateTableStatement(
    TableName TableName,
    bool IsTemp,
    bool IfNotExists,
    ColumnDef[] Columns,
    TableConstraint? Constraint = null,
    TableOptions? Options = null);
