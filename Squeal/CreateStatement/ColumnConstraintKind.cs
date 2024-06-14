namespace Squeal.CreateStatement;

public record ColumnConstraintKind(string Name, ColumnConstraints Type)
{
    public static ColumnConstraintKind Default => new(String.Empty, ColumnConstraints.Undefined);
};
