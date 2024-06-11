namespace Squeal.CreateStatement;

public record CreateTableStatement(TableName TableName, bool IsTemp, bool IfNotExists);
