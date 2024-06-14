namespace Squeal.CreateStatement;

public enum ConflictResolutions
{
    Undefined,
    Rollback,
    Abort,
    Fail,
    Ignore,
    Replace,
}
