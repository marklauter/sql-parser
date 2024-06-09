namespace Squeal.CreateStatement.Expressions;

internal enum ColumnTypes : uint
{
    Text = CreateStatementTokens.ColumnTypeText,
    Numeric = CreateStatementTokens.ColumnTypeNumeric,
    Integer = CreateStatementTokens.ColumnTypeInteger,
    Real = CreateStatementTokens.ColumnTypeReal,
    Blob = CreateStatementTokens.ColumnTypeBlob,
}
