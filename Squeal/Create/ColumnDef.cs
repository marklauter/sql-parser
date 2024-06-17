using Squeal.Create.ColumnConstraints;

namespace Squeal.Create;

public record ColumnDef(string Name, TypeName Type, IColumnConstraint[] Constraints);
