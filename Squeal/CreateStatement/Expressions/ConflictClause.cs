namespace Squeal.CreateStatement.Expressions;

internal enum ConflictClause
{
    Rollback,
    Abort,
    Fail,
    Ignore,
    Replace,
}
