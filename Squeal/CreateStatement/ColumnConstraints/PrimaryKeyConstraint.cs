namespace Squeal.CreateStatement.ColumnConstraints;

public sealed record PrimaryKeyConstraint(string Name, Order Order, ConflictResolutions Resolution, bool AutoIncrement)
    : ColumnConstraint(Name)
{
    public static PrimaryKeyConstraint Default() => Default(String.Empty);

    public static PrimaryKeyConstraint Default(string Name) =>
        new(Name, Order.Asc, ConflictResolutions.Default, true);
}

