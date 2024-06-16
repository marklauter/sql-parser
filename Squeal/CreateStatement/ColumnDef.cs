using Squeal.CreateStatement.ColumnConstraints;

namespace Squeal.CreateStatement;

public record ColumnDef(string Name, TypeName Type, IColumnConstraint[] Constraints);
