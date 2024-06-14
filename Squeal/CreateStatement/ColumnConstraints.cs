namespace Squeal.CreateStatement;

public enum ColumnConstraints
{
    Undefined,
    PrimaryKeyAsc,
    PrimaryKeyDesc,
    NotNull,
    Unique,
    Check,
    Default,
    Collate,
    Generated,
    As,
}
