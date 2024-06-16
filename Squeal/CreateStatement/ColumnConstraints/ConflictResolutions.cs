namespace Squeal.CreateStatement.ColumnConstraints;

public enum ConflictResolutions
{
    Default,
    Rollback,
    Abort,
    Fail,
    Ignore,
    Replace,
}
