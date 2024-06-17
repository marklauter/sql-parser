using Squeal.Select;

namespace Squeal.Tests;

public sealed class SelectStatementParserTests
{
    [Theory]
    [InlineData("SELECT name FROM apples", 1)]
    [InlineData("SELECT name, id FROM apples", 2)]
    public void ParseSelectStatement(string sql, int expectedColumnCount)
    {
        var result = Sql.TryParse(sql);
        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is SelectStatement);
        var statement = (SelectStatement)result.Value;
        Assert.Equal("apples", statement.TableName.Name);
        Assert.Equal(expectedColumnCount, statement.Columns.Length);
        Assert.Equal("name", statement.Columns[0].Name);
        if (expectedColumnCount == 2)
        {
            Assert.Equal("id", statement.Columns[1].Name);
        }
    }

    [Theory]
    [InlineData("SELECT * FROM apples", 1)]
    public void ParseSelectStarStatement(string sql, int expectedColumnCount)
    {
        var result = Sql.TryParse(sql);
        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is SelectStatement);
        var statement = (SelectStatement)result.Value;
        Assert.Equal("apples", statement.TableName.Name);
        Assert.Equal(expectedColumnCount, statement.Columns.Length);
        Assert.Equal("*", statement.Columns[0].Name);
    }

    [Theory]
    [InlineData("SELECT count(*) FROM apples")]
    public void ParseSelectCountStatement(string sql)
    {
        var result = Sql.TryParse(sql);
        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is SelectCountStatement);
        var statement = (SelectCountStatement)result.Value;
        Assert.Equal("apples", statement.TableName.Name);
    }
}
