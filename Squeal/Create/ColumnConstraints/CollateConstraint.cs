namespace Squeal.Create.ColumnConstraints;

public sealed record CollateConstraint(string Name, string CollationName)
    : ColumnConstraint(Name);

