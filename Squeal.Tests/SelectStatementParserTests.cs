using Squeal.Select;
using Squeal.Select.Expressions;
using Superpower;

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
        Assert.Equal("apples", statement.From.Name);
        Assert.Equal(expectedColumnCount, statement.Projection.Length);
        Assert.Equal("name", statement.Projection[0].Name);
        if (expectedColumnCount == 2)
        {
            Assert.Equal("id", statement.Projection[1].Name);
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
        Assert.Equal("apples", statement.From.Name);
        Assert.Equal(expectedColumnCount, statement.Projection.Length);
        Assert.Equal("*", statement.Projection[0].Name);
    }

    [Theory]
    [InlineData("SELECT count(*) FROM apples")]
    public void ParseSelectCountStatement(string sql)
    {
        var result = Sql.TryParse(sql);
        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is SelectCountStatement);
        var statement = (SelectCountStatement)result.Value;
        Assert.Equal("apples", statement.From.Name);
    }

    [Theory]
    [InlineData("color = 'Yellow'")]
    public void ParseConditionalExpression(string sql)
    {
        var result = Sql
            .ConditionalExp
            .TryParse(Sql.Tokenizer.Tokenize(sql));

        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is not null);

        Assert.True(result.Value is ConditionalExpression);
        var exp = (ConditionalExpression)result.Value;
        Assert.Equal(ConditionalOperators.EqualTo, exp.Operator);
        Assert.True(exp.Left is ColumnExpression);
        Assert.Equal("color", ((ColumnExpression)exp.Left).ColumnName);
        Assert.True(exp.Right is StringLiteralExpression);
        Assert.Equal("Yellow", ((StringLiteralExpression)exp.Right).Value);
    }

    [Theory]
    [InlineData("color = 'Yellow' and id = '5'")]
    public void ParseLogicallExpression(string sql)
    {
        var result = Sql
            .LogicalExp
            .TryParse(Sql.Tokenizer.Tokenize(sql));

        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is not null);

        Assert.True(result.Value is LogicalExpression);
        var exp = (LogicalExpression)result.Value;
        Assert.Equal(LogicalOperators.And, exp.Operator);
        Assert.True(exp.Left is ConditionalExpression);
        Assert.True(exp.Right is ConditionalExpression);

        var leftExp = (ConditionalExpression)exp.Left;
        Assert.True(leftExp.Left is ColumnExpression);
        Assert.Equal("color", ((ColumnExpression)leftExp.Left).ColumnName);
        Assert.True(leftExp.Right is StringLiteralExpression);
        Assert.Equal("Yellow", ((StringLiteralExpression)leftExp.Right).Value);

        var rightExp = (ConditionalExpression)exp.Right;
        Assert.True(rightExp.Left is ColumnExpression);
        Assert.Equal("id", ((ColumnExpression)rightExp.Left).ColumnName);
        Assert.True(rightExp.Right is StringLiteralExpression);
        Assert.Equal("5", ((StringLiteralExpression)rightExp.Right).Value);
    }

    [Theory]
    [InlineData("SELECT name, color FROM apples WHERE color = 'Yellow'")]
    [InlineData("SELECT name, color FROM apples WHERE color = 'Yellow' or color = 'Red'")]
    public void ParsePredicate(string sql)
    {
        var result = Sql.TryParse(sql);
        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is SelectStatement);
        var statement = (SelectStatement)result.Value;
        Assert.NotEqual(Predicate.Default, statement.Where);
    }
}
