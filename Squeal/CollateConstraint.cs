namespace Squeal;

public sealed record CollateConstraint(string Name, string CollationName)
    : ColumnConstraint(Name);

