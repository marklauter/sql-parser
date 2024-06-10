using Superpower;
using Superpower.Model;
using Superpower.Parsers;
namespace Squeal.Tests;

public static class DdlToken
{
    public static TextParser<TextSpan> Identifier { get; } = Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*");

    public static TextParser<TextSpan> Create { get; } =
        Span.WhiteSpace.Optional()
        .IgnoreThen(Span.EqualToIgnoreCase("CREATE"))
        .IgnoreThen(Span.WhiteSpace);

    public static TextParser<bool> IsTemporary { get; } =
        Span.EqualToIgnoreCase("TEMPORARY").Try()
        .Or(Span.EqualToIgnoreCase("TEMP")).Try()
        .Optional().Then(ts => Parse.Return(ts.HasValue));

    public static TextParser<TextSpan> Table { get; } =
        Span.WhiteSpace.Optional()
        .IgnoreThen(Span.EqualToIgnoreCase("TABLE"))
        .IgnoreThen(Span.WhiteSpace);

    public static TextParser<TableName> TableName { get; } =
        Identifier.Then(first =>
            Character.EqualTo('.').Optional().Then(
                dot => dot.HasValue
                    ? Identifier.Then(second => Parse.Return(new TableName(second.ToString(), first.ToString())))
                    : Parse.Return(new TableName(first.ToString(), null))));

    public static TextParser<CreateTableStatement> CreateTableStatement { get; } =
        Create.IgnoreThen(IsTemporary.Then(isTemporary =>
            Table.IgnoreThen(TableName.Then(tableName =>
            Parse.Return(new CreateTableStatement(tableName, isTemporary))))));

    //public static TextParser<bool> CheckExists { get; } = Span.EqualToIgnoreCase("IF NOT EXISTS").IgnoreThen(Span.WhiteSpace);
    //public static TextParser<TextSpan> OpenParen { get; } = Span.EqualTo('(').IgnoreThen(Span.WhiteSpace);
    //public static TextParser<TextSpan> CloseParen { get; } = Span.EqualTo(')').IgnoreThen(Span.WhiteSpace);
    //public static TextParser<TextSpan> Comma { get; } = Span.EqualTo(',').IgnoreThen(Span.WhiteSpace);
    //public static TextParser<TextSpan> PrimaryKey { get; } = Span.EqualToIgnoreCase("PRIMARY KEY").IgnoreThen(Span.WhiteSpace);
    //public static TextParser<TextSpan> Unique { get; } = Span.EqualToIgnoreCase("UNIQUE").IgnoreThen(Span.WhiteSpace);
}

public record TableName(string Name, string? Schema);

public record CreateTableStatement(TableName TableName, bool IsTemp);

public record TypeName(string Name, int[] Modifier);

public record ColumnDef(string Name, TypeName DataType, bool IsPrimaryKey, bool IsAutoIncrement);

public sealed class CreateTableParserTests
{
    private static readonly string CreateTableDdl = File.ReadAllText("create-table-apples.sql");
    private static readonly string CreateTempTableDdl = File.ReadAllText("create-temp-table-apples.sql");
    private static readonly string CreateSchemaTableDdl = File.ReadAllText("create-table-trees-apples.sql");

    [Fact]
    public void CreateTest()
    {
        var result = DdlToken.Create.TryParse(CreateTableDdl);
        Assert.True(result.HasValue, "result.HasValue");
    }

    [Theory]
    [InlineData("table", false)]
    [InlineData("temp", true)]
    [InlineData("temporary", true)]
    public void IsTemporaryTest(string ddl, bool isTemp)
    {
        var result = DdlToken.IsTemporary.TryParse(ddl);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(isTemp, result.Value);
    }

    [Theory]
    [InlineData("create table apples", false)]
    [InlineData("create temp table apples", true)]
    [InlineData("create temporary table apples", true)]
    public void CreateThenIsTemporaryAndNameTest(string ddl, bool isTemporary)
    {
        var p = DdlToken.Create.IgnoreThen(
            DdlToken.IsTemporary.Then(isTemporary =>
                DdlToken.Table.IgnoreThen(DdlToken.TableName
                    .Then(name => Parse.Return((isTemporary, name))))));

        var result = p.TryParse(ddl);
        //var result = DdlToken.Create.IgnoreThen(DdlToken.IsTemporary).TryParse(ddl);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(isTemporary, result.Value.isTemporary);
    }

    [Fact]
    public void CreateTableStatementTest()
    {
        var result = DdlToken.CreateTableStatement.TryParse(CreateTableDdl);
        Assert.True(result.HasValue, "result.HasValue");
        var createTable = result.Value;
        Assert.Equal("apples", createTable.TableName.Name);
        Assert.Null(createTable.TableName.Schema);
        Assert.False(createTable.IsTemp, "IsTemp");
    }

    [Fact]
    public void CreateTempTableStatementTest()
    {
        var result = DdlToken.CreateTableStatement.TryParse(CreateTempTableDdl);
        Assert.True(result.HasValue, "result.HasValue");
        var createTable = result.Value;
        Assert.Equal("apples", createTable.TableName.Name);
        Assert.Null(createTable.TableName.Schema);
        Assert.True(createTable.IsTemp, "IsTemp");
    }

    [Fact]
    public void CreateSchemaTableStatementTest()
    {
        var result = DdlToken.CreateTableStatement.TryParse(CreateSchemaTableDdl);
        Assert.True(result.HasValue, "result.HasValue");
        var createTable = result.Value;
        Assert.Equal("apples", createTable.TableName.Name);
        Assert.Equal("trees", createTable.TableName.Schema);
        Assert.False(createTable.IsTemp, "IsTemp");
    }
}
