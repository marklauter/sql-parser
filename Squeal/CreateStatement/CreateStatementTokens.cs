namespace Squeal.CreateStatement;

internal enum CreateStatementTokens : uint
{
    Ignore = 0,
    Temporary,
    Table,
    If,
    Not,
    Exists,
    Dot,
    ColumnTypeText,
    ColumnTypeNumeric,
    ColumnTypeInteger,
    ColumnTypeReal,
    ColumnTypeBlob,
    Identifier,
    OpenParens,
    CloseParens,
    Comma,
}
