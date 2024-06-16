namespace Squeal.CreateStatement.ColumnConstraints;

public sealed record UniqueConstraint(string Name, ConflictResolutions Resolution)
    : ColumnConstraint(Name);

