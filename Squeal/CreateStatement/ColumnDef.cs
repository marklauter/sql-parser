namespace Squeal.CreateStatement;

public record TypeName(string Name, int[] Modifier);

public record ColumnConstraint(string? Name);

public record ColumnDef(string Name, TypeName? Type, ColumnConstraint? Constraint);
