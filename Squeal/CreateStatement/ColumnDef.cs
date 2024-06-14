namespace Squeal.CreateStatement;

public record ColumnDef(string Name, TypeName Type, ColumnConstraintKind? Constraint);
