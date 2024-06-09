namespace Squeal;

internal enum StatementTokens : uint
{
    Ignore = 0,
    Create,
    Select,
    Alter,
    Drop,
    Insert,
    Update,
    Delete,
}
