using Superpower;
using Superpower.Model;
using Superpower.Parsers;
namespace Squeal.Tests;

public enum ColumnTypes : uint
{
    Text,
    Numeric,
    Integer,
    Real,
    Blob,
}

public static class DdlToken
{
    public static TextParser<string> Identifier { get; } = Span.Regex(@"[a-zA-Z_][a-zA-Z0-9_]*").Then(ts => Parse.Return(ts.ToString()));

    public static TextParser<TextSpan> Create { get; } =
        Span.WhiteSpace.Optional()
        .IgnoreThen(Span.EqualToIgnoreCase("CREATE"))
        .IgnoreThen(Span.WhiteSpace);

    public static TextParser<bool> IsTemporary { get; } =
        Span.EqualToIgnoreCase("TEMPORARY").Try().Or(Span.EqualToIgnoreCase("TEMP").Try())
        .IgnoreThen(Span.WhiteSpace)
        .Value(true)
        .OptionalOrDefault(false);

    public static TextParser<TextSpan> Table { get; } =
        Span.EqualToIgnoreCase("TABLE")
        .IgnoreThen(Span.WhiteSpace);

    public static TextParser<TableName> TableName { get; } =
        Identifier.Then(first =>
            Character.EqualTo('.').Optional().Then(dot => dot.HasValue
                ? Identifier.Then(second => Parse.Return(new TableName(second, first)))
                : Parse.Return(new TableName(first, null))));

    public static TextParser<bool> IfNotExists { get; } =
        Span.EqualToIgnoreCase("IF NOT EXISTS").IgnoreThen(Span.WhiteSpace).Try()
        .Value(true)
        .OptionalOrDefault(false);

    public static TextParser<CreateTableStatement> CreateTableStatement { get; } =
        Create.IgnoreThen(IsTemporary.Then(isTemporary =>
            Table.IgnoreThen(IfNotExists.Then(ifNotExists =>
                TableName.Then(tableName =>
                    Parse.Return(new CreateTableStatement(tableName, isTemporary, ifNotExists)))))));

    public static TextParser<TextSpan> OpenParen { get; } =
        Span.WhiteSpace.Optional()
        .IgnoreThen(Span.EqualTo('('));

    public static TextParser<TextSpan> CloseParen { get; } = Span.EqualTo(')');
    //Span.WhiteSpace.Optional().Try()
    //.IgnoreThen(Span.EqualTo(')'));

    public static TextParser<TextSpan> Comma { get; } = Span.EqualTo(',');
    public static TextParser<TextSpan?> CommaDelimiter { get; } =
        Span.WhiteSpace.Optional()
        .IgnoreThen(Comma)
        .IgnoreThen(Span.WhiteSpace.Optional());

    public static TextParser<ColumnTypes> ColumnType { get; } =
        Span.EqualToIgnoreCase("TEXT").Try().Value(ColumnTypes.Text)
        .Or(Span.EqualToIgnoreCase("NUMERIC").Try().Or(Span.EqualToIgnoreCase("NUM").Try()).Value(ColumnTypes.Numeric))
        .Or(Span.EqualToIgnoreCase("INTEGER").Try().Or(Span.EqualToIgnoreCase("INT").Try()).Value(ColumnTypes.Integer))
        .Or(Span.EqualToIgnoreCase("REAL").Value(ColumnTypes.Real))
        .Or(Span.EqualToIgnoreCase("BLOB").Value(ColumnTypes.Blob))
        .OptionalOrDefault(ColumnTypes.Blob);

    public static TextParser<ColumnDef> Column { get; } =
        Span.WhiteSpace.Optional().IgnoreThen(Identifier.Then(name =>
            Span.WhiteSpace.IgnoreThen(ColumnType.Then(type =>
                Span.WhiteSpace.Optional().Value(new ColumnDef(name, type, false, false))))));

    public static TextParser<ColumnDef[]> Columns { get; } =
        OpenParen
        .IgnoreThen(Column.ManyDelimitedBy(Comma))
        .Then(columns => CloseParen.Value(columns));

    //public static TextParser<TextSpan> PrimaryKey { get; } = Span.EqualToIgnoreCase("PRIMARY KEY").IgnoreThen(Span.WhiteSpace);
    //public static TextParser<TextSpan> Unique { get; } = Span.EqualToIgnoreCase("UNIQUE").IgnoreThen(Span.WhiteSpace);
}

public record TableName(string Name, string? Schema);

public record CreateTableStatement(TableName TableName, bool IsTemp, bool IfNotExists);

//public record DataType(DataTypes Type, int[] Modifier);

public record ColumnDef(string Name, ColumnTypes Type, bool IsPrimaryKey, bool IsAutoIncrement);

public sealed class CreateTableParserTests
{
    private static readonly string CreateTableDdl = File.ReadAllText("create-table-apples.sql");
    private static readonly string CreateTableSimpleColumnsDdl = File.ReadAllText("create-table-apples-simple-columns.sql");
    private static readonly string CreateTempTableDdl = File.ReadAllText("create-temp-table-apples.sql");
    private static readonly string CreateSchemaTableDdl = File.ReadAllText("create-table-trees-apples.sql");
    private static readonly string CreateTableIfNotExistsDdl = File.ReadAllText("create-table-ifnotexists-apples.sql");

    [Fact]
    public void CreateTest()
    {
        var result = DdlToken.Create.TryParse(CreateTableDdl);
        Assert.True(result.HasValue, "result.HasValue");
    }

    [Theory]
    [InlineData("table", false)]
    [InlineData("temp ", true)]
    [InlineData("temporary ", true)]
    public void IsTemporaryTest(string ddl, bool isTemp)
    {
        var result = DdlToken.IsTemporary.TryParse(ddl);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(isTemp, result.Value);
    }

    [Theory]
    [InlineData("TEXT", ColumnTypes.Text)]
    [InlineData("NUMERIC", ColumnTypes.Numeric)]
    [InlineData("NUM", ColumnTypes.Numeric)]
    [InlineData("INTEGER", ColumnTypes.Integer)]
    [InlineData("INT", ColumnTypes.Integer)]
    [InlineData("REAL", ColumnTypes.Real)]
    [InlineData("BLOB", ColumnTypes.Blob)]
    [InlineData("", ColumnTypes.Blob)]
    public void ColumnTypeTest(string ddl, ColumnTypes type)
    {
        var result = DdlToken.ColumnType.TryParse(ddl);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(type, result.Value);
    }

    [Theory]
    [InlineData("create table apples", false)]
    [InlineData("create temp table apples", true)]
    [InlineData("create temporary table apples", true)]
    public void CreateThenIsTemporaryAndNameTest(string ddl, bool isTemporary)
    {
        var parser = DdlToken.Create.IgnoreThen(
            DdlToken.IsTemporary.Then(isTemporary =>
                DdlToken.Table.IgnoreThen(DdlToken.TableName
                    .Then(name => Parse.Return((isTemporary, name))))));

        var result = parser.TryParse(ddl);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(isTemporary, result.Value.isTemporary);
    }

    [Theory]
    [InlineData("(id integer,name text,color text)")]
    [InlineData("( id integer,name text,color text)")]
    [InlineData("(id integer,name text,color text )")]
    [InlineData(" (id integer,name text,color text)")]
    [InlineData("(id integer ,name text , color text)")]
    [InlineData("(id integer, name text, color text)")]
    public void ColumnsTest(string ddl)
    {
        var result = DdlToken.Columns.TryParse(ddl);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(3, result.Value.Length);
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
        Assert.False(createTable.IfNotExists, "IfNotExists");
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
        Assert.False(createTable.IfNotExists, "IfNotExists");
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
        Assert.False(createTable.IfNotExists, "IfNotExists");
    }

    [Fact]
    public void CreateTableIfNotExistsStatementTest()
    {
        var result = DdlToken.CreateTableStatement.TryParse(CreateTableIfNotExistsDdl);
        Assert.True(result.HasValue, "result.HasValue");
        var createTable = result.Value;
        Assert.Equal("apples", createTable.TableName.Name);
        Assert.Null(createTable.TableName.Schema);
        Assert.False(createTable.IsTemp, "IsTemp");
        Assert.True(createTable.IfNotExists, "IfNotExists");
    }
}
