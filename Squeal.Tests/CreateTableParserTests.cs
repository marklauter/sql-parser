using Superpower;

namespace Squeal.Tests;

public sealed class CreateTableParserTests
{
    //private static readonly string CreateTableDdl = File.ReadAllText("create-table-apples.sql");
    //private static readonly string CreateTableSimpleColumnsDdl = File.ReadAllText("create-table-apples-simple-columns.sql");
    //private static readonly string CreateTempTableDdl = File.ReadAllText("create-temp-table-apples.sql");
    //private static readonly string CreateSchemaTableDdl = File.ReadAllText("create-table-trees-apples.sql");
    //private static readonly string CreateTableIfNotExistsDdl = File.ReadAllText("create-table-ifnotexists-apples.sql");

    [Fact]
    public void Test()
    {
        var sql = "create temp table if not exists trees.apples";
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Ddl.CreateTableStatement.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var statement = result.Value;
        Assert.True(statement.IsTemp, "IsTemp");
        Assert.True(statement.IfNotExists, "IfNotExists");
        Assert.Equal("apples", statement.TableName.Name);
        Assert.Equal("trees", statement.TableName.Schema);
    }

    [Fact]
    public void Test2()
    {
        var sql = "create table apples";
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Ddl.CreateTableStatement.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var statement = result.Value;
        Assert.False(statement.IsTemp, "IsTemp");
        Assert.False(statement.IfNotExists, "IfNotExists");
        Assert.Equal("apples", statement.TableName.Name);
        Assert.Null(statement.TableName.Schema);
    }

    [Theory]
    [InlineData("name", 0)]
    [InlineData("name(1)", 1)]
    [InlineData("name(1,2)", 2)]
    public void TypeNameTest(string sql, int expectedCount)
    {
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Ddl.TypeName.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var typeName = result.Value;
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
    [InlineData("name(1")]
    public void TypeNameReturnsUnexpectedEndOfInput(string sql)
    {
        var tokens = Sql.Tokenizer.Tokenize(sql);
        var result = Ddl.TypeName.TryParse(tokens);
        Assert.False(result.HasValue, result.ToString());
        Assert.Contains("unexpected end of input", result.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    //[Fact]
    //public void CreateTest()
    //{
    //    var result = CreateStatement.CreateDDL.Create.TryParse(CreateTableDdl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //}

    //[Theory]
    //[InlineData("table", false)]
    //[InlineData("temp ", true)]
    //[InlineData("temporary ", true)]
    //public void IsTemporaryTest(string ddl, bool isTemp)
    //{
    //    var result = CreateStatement.CreateDDL.IsTemporary.TryParse(ddl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    Assert.Equal(isTemp, result.Value);
    //}

    //[Theory]
    //[InlineData("TEXT", ColumnTypes.Text)]
    //[InlineData("NUMERIC", ColumnTypes.Numeric)]
    //[InlineData("NUM", ColumnTypes.Numeric)]
    //[InlineData("INTEGER", ColumnTypes.Integer)]
    //[InlineData("INT", ColumnTypes.Integer)]
    //[InlineData("REAL", ColumnTypes.Real)]
    //[InlineData("BLOB", ColumnTypes.Blob)]
    //[InlineData("", ColumnTypes.Blob)]
    //public void ColumnTypeTest(string ddl, ColumnTypes type)
    //{
    //    var result = CreateStatement.CreateDDL.ColumnType.TryParse(ddl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    Assert.Equal(type, result.Value);
    //}

    //[Theory]
    //[InlineData("create table apples", false)]
    //[InlineData("create temp table apples", true)]
    //[InlineData("create temporary table apples", true)]
    //public void CreateThenIsTemporaryAndNameTest(string ddl, bool isTemporary)
    //{
    //    var parser = CreateStatement.CreateDDL.Create.IgnoreThen(
    //        CreateStatement.CreateDDL.IsTemporary.Then((object isTemporary) =>
    //            CreateStatement.CreateDDL.Table.IgnoreThen(CreateStatement.CreateDDL.TableName
    //                .Then((object name) => Parse.Return((isTemporary, name))))));

    //    var result = parser.TryParse(ddl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    Assert.Equal(isTemporary, result.Value.isTemporary);
    //}

    //[Theory]
    //[InlineData("(id integer,name text,color text)")]
    //[InlineData("( id integer,name text,color text)")]
    //[InlineData("(id integer,name text,color text )")]
    //[InlineData(" (id integer,name text,color text)")]
    //[InlineData("(id integer ,name text , color text)")]
    //[InlineData("(id integer, name text, color text)")]
    //public void ColumnsTest(string ddl)
    //{
    //    var result = CreateStatement.CreateDDL.Columns.TryParse(ddl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    Assert.Equal(3, result.Value.Length);
    //}

    //[Fact]
    //public void CreateTableStatementTest()
    //{
    //    var result = CreateStatement.CreateDDL.CreateTableStatement.TryParse(CreateTableDdl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    var createTable = result.Value;
    //    Assert.Equal("apples", createTable.TableName.Name);
    //    Assert.Null(createTable.TableName.Schema);
    //    Assert.False(createTable.IsTemp, "IsTemp");
    //    Assert.False(createTable.IfNotExists, "IfNotExists");
    //}

    //[Fact]
    //public void CreateTempTableStatementTest()
    //{
    //    var result = CreateStatement.CreateDDL.CreateTableStatement.TryParse(CreateTempTableDdl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    var createTable = result.Value;
    //    Assert.Equal("apples", createTable.TableName.Name);
    //    Assert.Null(createTable.TableName.Schema);
    //    Assert.True(createTable.IsTemp, "IsTemp");
    //    Assert.False(createTable.IfNotExists, "IfNotExists");
    //}

    //[Fact]
    //public void CreateSchemaTableStatementTest()
    //{
    //    var result = CreateStatement.CreateDDL.CreateTableStatement.TryParse(CreateSchemaTableDdl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    var createTable = result.Value;
    //    Assert.Equal("apples", createTable.TableName.Name);
    //    Assert.Equal("trees", createTable.TableName.Schema);
    //    Assert.False(createTable.IsTemp, "IsTemp");
    //    Assert.False(createTable.IfNotExists, "IfNotExists");
    //}

    //[Fact]
    //public void CreateTableIfNotExistsStatementTest()
    //{
    //    var result = CreateStatement.CreateDDL.CreateTableStatement.TryParse(CreateTableIfNotExistsDdl);
    //    Assert.True(result.HasValue, "result.HasValue");
    //    var createTable = result.Value;
    //    Assert.Equal("apples", createTable.TableName.Name);
    //    Assert.Null(createTable.TableName.Schema);
    //    Assert.False(createTable.IsTemp, "IsTemp");
    //    Assert.True(createTable.IfNotExists, "IfNotExists");
    //}
}
