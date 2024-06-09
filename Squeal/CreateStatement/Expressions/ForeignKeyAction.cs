namespace Squeal.CreateStatement.Expressions;

internal enum ForeignKeyAction
{
    NoAction,
    SetNull,
    SetDefault,
    Cascade,
    Restrict,
}
