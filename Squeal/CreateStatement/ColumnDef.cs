namespace Squeal.CreateStatement;

public record ColumnDef(string Name, TypeName? Type, ColumnConstraint? Constraint);
