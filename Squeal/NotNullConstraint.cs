namespace Squeal;

public sealed record NotNullConstraint(string Name, ConflictResolutions Resolution)
    : ColumnConstraint(Name)
{
    public static NotNullConstraint Default() => Default(String.Empty);

    public static NotNullConstraint Default(string Name) => new(Name, ConflictResolutions.Default);
}
