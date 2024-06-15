namespace Squeal.CreateStatement;

public sealed record PrimaryKey(Order Order, ConflictResolutions Resolution, bool AutoIncrement);
