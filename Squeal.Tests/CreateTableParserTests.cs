namespace Squeal.Tests;

public class CreateTableParserTests
{
    private static readonly string sql = File.ReadAllText("create-table-apples.sql");

    [Fact]
    public void ParseReturnsAST()
    {
        var statement = SqlParser.Parse<>(sql);
        Assert.Equal("", statement.)
    }
}
