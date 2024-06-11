namespace Squeal.CreateStatement;

//public record DataType(DataTypes Type, int[] Modifier);

public record ColumnDef(string Name, int Type, bool IsPrimaryKey, bool IsAutoIncrement);
