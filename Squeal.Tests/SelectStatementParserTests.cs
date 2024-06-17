using Superpower;
using Superpower.Model;

namespace Squeal.Tests;

public sealed class SelectStatementParserTests
{
    [Fact]
    public void ParseSelectStatement()
    {
        var sql = "SELECT name FROM apples";
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Sql.SelectStatement.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
    }
}
