namespace Squeal.Select;

public record TableName(string Name, string? Schema)
{
    public override string ToString() => Schema is null ? Name : $"{Schema}.{Name}";
}
