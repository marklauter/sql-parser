using Squeal.CreateStatement;
using Superpower;

namespace Squeal.Tests;

public sealed class CreateTableParserTests
{
    [Theory]
    [ClassData(typeof(DdlTestData))]
    public void CreateTableStatementTest(string ddl, bool isTemp, bool ifNotExists, string tableName, string? schema)
    {
        var tokens = Sql.Tokenizer.Tokenize(ddl);
        var result = Ddl.CreateTableStatement.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var statement = result.Value;
        Assert.Equal(isTemp, statement.IsTemp);
        Assert.Equal(ifNotExists, statement.IfNotExists);
        Assert.Equal(tableName, statement.TableName.Name);
        Assert.Equal(schema, statement.TableName.Schema);
    }

    [Theory]
    [InlineData("TEXT", ColumnTypes.TEXT, 0)]
    [InlineData("TEXT(1)", ColumnTypes.TEXT, 1)]
    [InlineData("TEXT(1, 2)", ColumnTypes.TEXT, 2)]
    [InlineData("NUMERIC", ColumnTypes.NUMERIC, 0)]
    [InlineData("NUMERIC(1)", ColumnTypes.NUMERIC, 1)]
    [InlineData("NUMERIC(1, 2)", ColumnTypes.NUMERIC, 2)]
    [InlineData("NUM", ColumnTypes.NUMERIC, 0)]
    [InlineData("NUM(1)", ColumnTypes.NUMERIC, 1)]
    [InlineData("NUM(1, 2)", ColumnTypes.NUMERIC, 2)]
    [InlineData("INTEGER", ColumnTypes.INTEGER, 0)]
    [InlineData("INTEGER(1)", ColumnTypes.INTEGER, 1)]
    [InlineData("INTEGER(1, 2)", ColumnTypes.INTEGER, 2)]
    [InlineData("INT", ColumnTypes.INTEGER, 0)]
    [InlineData("INT(1)", ColumnTypes.INTEGER, 1)]
    [InlineData("INT(1, 2)", ColumnTypes.INTEGER, 2)]
    [InlineData("REAL", ColumnTypes.REAL, 0)]
    [InlineData("REAL(1)", ColumnTypes.REAL, 1)]
    [InlineData("REAL(1, 2)", ColumnTypes.REAL, 2)]
    [InlineData("BLOB", ColumnTypes.BLOB, 0)]
    [InlineData("BLOB(1)", ColumnTypes.BLOB, 1)]
    [InlineData("BLOB(1, 2)", ColumnTypes.BLOB, 2)]
    [InlineData("", ColumnTypes.BLOB, 0)]
    public void TypeNameTest(string ddl, ColumnTypes expectedType, int expectedCount)
    {
        var tokens = Sql.Tokenizer.Tokenize(ddl);
        var result = Ddl.TypeName.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var typeName = result.Value;
        Assert.Equal(expectedType, typeName.Type);
        Assert.Equal(expectedCount, typeName.Modifier.Length);
        if (expectedCount > 0)
        {
            Assert.Equal(1, typeName.Modifier.First());
        }

        if (expectedCount > 1)
        {
            Assert.Equal(2, typeName.Modifier.Last());
        }
    }

    [Theory]
    [InlineData("INTEGER(1")]
    public void TypeNameReturnsUnexpectedEndOfInput(string sql)
    {
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Ddl.TypeName.TryParse(tokens);
        Assert.False(result.HasValue, result.ToString());
        Assert.Contains("unexpected end of input", result.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("table", false)]
    [InlineData("temp", true)]
    [InlineData("temporary", true)]
    public void IsTemporaryTest(string ddl, bool isTemp)
    {
        var tokens = Sql.Tokenizer.Tokenize(ddl);
        var result = Ddl.IsTemporary.TryParse(tokens);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(isTemp, result.Value);
    }

    [Theory]
    [InlineData("Id TEXT", ColumnTypes.TEXT, 0)]
    [InlineData("Id TEXT(1)", ColumnTypes.TEXT, 1)]
    [InlineData("Id TEXT(1, 2)", ColumnTypes.TEXT, 2)]
    [InlineData("Id NUMERIC", ColumnTypes.NUMERIC, 0)]
    [InlineData("Id NUMERIC(1)", ColumnTypes.NUMERIC, 1)]
    [InlineData("Id NUMERIC(1, 2)", ColumnTypes.NUMERIC, 2)]
    [InlineData("Id NUM", ColumnTypes.NUMERIC, 0)]
    [InlineData("Id NUM(1)", ColumnTypes.NUMERIC, 1)]
    [InlineData("Id NUM(1, 2)", ColumnTypes.NUMERIC, 2)]
    [InlineData("Id INTEGER", ColumnTypes.INTEGER, 0)]
    [InlineData("Id INTEGER(1)", ColumnTypes.INTEGER, 1)]
    [InlineData("Id INTEGER(1, 2)", ColumnTypes.INTEGER, 2)]
    [InlineData("Id INT", ColumnTypes.INTEGER, 0)]
    [InlineData("Id INT(1)", ColumnTypes.INTEGER, 1)]
    [InlineData("Id INT(1, 2)", ColumnTypes.INTEGER, 2)]
    [InlineData("Id REAL", ColumnTypes.REAL, 0)]
    [InlineData("Id REAL(1)", ColumnTypes.REAL, 1)]
    [InlineData("Id REAL(1, 2)", ColumnTypes.REAL, 2)]
    [InlineData("Id BLOB", ColumnTypes.BLOB, 0)]
    [InlineData("Id BLOB(1)", ColumnTypes.BLOB, 1)]
    [InlineData("Id BLOB(1, 2)", ColumnTypes.BLOB, 2)]
    [InlineData("Id", ColumnTypes.BLOB, 0)]
    public void ColumnTest(string sql, ColumnTypes expectedType, int expectedCount)
    {
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Ddl.Column.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var column = result.Value;
        Assert.Equal("Id", column.Name);

        Assert.NotNull(column.Type);
        var typeName = column.Type;
        Assert.Equal(expectedType, typeName.Type);
        Assert.Equal(expectedCount, typeName.Modifier.Length);
        if (expectedCount > 0)
        {
            Assert.Equal(1, typeName.Modifier.First());
        }

        if (expectedCount > 1)
        {
            Assert.Equal(2, typeName.Modifier.Last());
        }
    }

    [Theory]
    [InlineData("(id integer,name text,color num)")]
    [InlineData("( id integer,name text,color num)")]
    [InlineData("(id integer,name text,color num )")]
    [InlineData(" (id integer,name text,color num)")]
    [InlineData("(id integer ,name text , color num)")]
    [InlineData("(id integer, name text, color num)")]
    public void ColumnsTest(string ddl)
    {
        var tokens = Sql.Tokenizer.Tokenize(ddl);
        var result = Ddl.Columns.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var columns = result.Value;
        Assert.Equal(3, columns.Length);
        Assert.Equal("id", columns[0].Name);
        Assert.Equal("name", columns[1].Name);
        Assert.Equal("color", columns[2].Name);
        Assert.Equal(ColumnTypes.INTEGER, columns[0].Type?.Type);
        Assert.Equal(ColumnTypes.TEXT, columns[1].Type?.Type);
        Assert.Equal(ColumnTypes.NUMERIC, columns[2].Type?.Type);
    }

    [Theory]
    [InlineData("primary key", "", ColumnConstraints.PrimaryKeyAsc)]
    [InlineData("CONSTRAINT cname primary key", "cname", ColumnConstraints.PrimaryKeyAsc)]
    [InlineData("primary key asc", "", ColumnConstraints.PrimaryKeyAsc)]
    [InlineData("CONSTRAINT cname primary key asc", "cname", ColumnConstraints.PrimaryKeyAsc)]
    [InlineData("primary key desc", "", ColumnConstraints.PrimaryKeyDesc)]
    [InlineData("CONSTRAINT cname primary key desc", "cname", ColumnConstraints.PrimaryKeyDesc)]
    [InlineData("not null", "", ColumnConstraints.NotNull)]
    [InlineData("CONSTRAINT cname not null", "cname", ColumnConstraints.NotNull)]
    [InlineData("unique", "", ColumnConstraints.Unique)]
    [InlineData("CONSTRAINT cname unique", "cname", ColumnConstraints.Unique)]
    [InlineData("check", "", ColumnConstraints.Check)]
    [InlineData("CONSTRAINT cname check", "cname", ColumnConstraints.Check)]
    [InlineData("default", "", ColumnConstraints.Default)]
    [InlineData("CONSTRAINT cname default", "cname", ColumnConstraints.Default)]
    [InlineData("collate", "", ColumnConstraints.Collate)]
    [InlineData("CONSTRAINT cname collate", "cname", ColumnConstraints.Collate)]
    [InlineData("generated always", "", ColumnConstraints.Generated)]
    [InlineData("CONSTRAINT cname generated always", "cname", ColumnConstraints.Generated)]
    public void ColumnConstraintKindTest(string ddl, string expectedName, ColumnConstraints expectedType)
    {
        var tokens = Sql.Tokenizer.Tokenize(ddl);
        var result = Ddl.ConstraintKind.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var constraint = result.Value;
        Assert.Equal(expectedName, constraint.Name);
        Assert.Equal(expectedType, constraint.Type);
    }
}
