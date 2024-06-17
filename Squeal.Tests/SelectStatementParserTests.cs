using Superpower;
using Superpower.Model;

namespace Squeal.Tests;

public sealed class SelectStatementParserTests
{
    [Theory]
    [InlineData("SELECT name FROM apples", 1)]
    [InlineData("SELECT name, id FROM apples", 2)]
    public void ParseSelectStatement(string sql, int expectedColumnCount)
    {
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Sql.SelectStatement.TryParse(tokens);
        var statement = result.Value;
        Assert.True(result.HasValue, result.ToString());
        Assert.Equal("apples", statement.TableName.Name);
        Assert.Equal(expectedColumnCount, statement.Columns.Length);
        Assert.Equal("name", statement.Columns[0].Name);
        if(expectedColumnCount == 2)
        {
            Assert.Equal("id", statement.Columns[1].Name);
        }
    }
}
