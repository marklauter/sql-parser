namespace Squeal;

public record ColumnDef(string Name, TypeName Type, IColumnConstraint[] Constraints);
