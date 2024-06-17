namespace Squeal.Select;

public record SelectCountStatement(TableName TableName)
    : ISelectStatement;
