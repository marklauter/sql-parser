namespace Squeal;

public sealed record UniqueConstraint(string Name, ConflictResolutions Resolution)
    : ColumnConstraint(Name);

