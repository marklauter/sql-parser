using Squeal.CreateStatement;
using Squeal.CreateStatement.ColumnConstraints;
using Superpower;

namespace Squeal.Tests;

public sealed class CreateTableParserTests
{
    [Theory]
    [ClassData(typeof(DdlTestData))]
    public void CreateTableStatementTest(string ddl, bool isTemp, bool ifNotExists, string tableName, string? schema)
    {
        var tokens = Ddl.Tokenizer.Tokenize(ddl);
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
    public void ColumnTypeNameTest(string ddl, ColumnTypes expectedType, int expectedCount)
    {
        var tokens = Ddl.Tokenizer.Tokenize(ddl);
        var result = Ddl.ColumnTypeName.TryParse(tokens);
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
    public void ColumnTypeNameReturnsUnexpectedEndOfInput(string sql)
    {
        var tokens = Ddl.Tokenizer.Tokenize(sql);
        var result = Ddl.ColumnTypeName.TryParse(tokens);
        Assert.False(result.HasValue, result.ToString());
        Assert.Contains("unexpected end of input", result.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("table", false)]
    [InlineData("temp", true)]
    [InlineData("temporary", true)]
    public void IsTemporaryTest(string ddl, bool isTemp)
    {
        var tokens = Ddl.Tokenizer.Tokenize(ddl);
        var result = Ddl.IsTemporary.TryParse(tokens);
        Assert.True(result.HasValue, "result.HasValue");
        Assert.Equal(isTemp, result.Value);
    }

    [Theory]
    [InlineData("", ConflictResolutions.Default)]
    [InlineData("on conflict rollback", ConflictResolutions.Rollback)]
    [InlineData("on conflict abort", ConflictResolutions.Abort)]
    [InlineData("on conflict fail", ConflictResolutions.Fail)]
    [InlineData("on conflict ignore", ConflictResolutions.Ignore)]
    [InlineData("on conflict replace", ConflictResolutions.Replace)]
    public void ConflictClauseTest(string ddl, ConflictResolutions expected)
    {
        var tokens = Ddl.Tokenizer.Tokenize(ddl);
        var result = Ddl.ConcflictClause.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [InlineData("primary key", Order.Asc, ConflictResolutions.Default, false)]
    [InlineData("primary key asc", Order.Asc, ConflictResolutions.Default, false)]
    [InlineData("primary key desc", Order.Desc, ConflictResolutions.Default, false)]
    [InlineData("primary key AUTOINCREMENT", Order.Asc, ConflictResolutions.Default, true)]
    [InlineData("primary key asc AUTOINCREMENT", Order.Asc, ConflictResolutions.Default, true)]
    [InlineData("primary key desc AUTOINCREMENT", Order.Desc, ConflictResolutions.Default, true)]
    [InlineData("primary key desc on conflict rollback autoincrement", Order.Desc, ConflictResolutions.Rollback, true)]
    [InlineData("primary key desc on conflict abort autoincrement", Order.Desc, ConflictResolutions.Abort, true)]
    public void PrimaryKeyTest(
        string ddl,
        Order expectedOrder,
        ConflictResolutions expectedResolution,
        bool expectedAutoInc)
    {
        var tokens = Ddl.Tokenizer.Tokenize(ddl);
        var result = Ddl.PrimaryKey("pk").TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        Assert.True(result.Value is PrimaryKeyConstraint);
        var primaryKey = (PrimaryKeyConstraint)result.Value;
        Assert.Equal(expectedOrder, primaryKey.Order);
        Assert.Equal(expectedResolution, primaryKey.Resolution);
        Assert.Equal(expectedAutoInc, primaryKey.AutoIncrement);
    }

    public static IEnumerable<object[]> ColumnDefData => new List<object[]>
    {
        new object[] { "Id TEXT", ColumnTypes.TEXT, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id TEXT(1)", ColumnTypes.TEXT, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id TEXT(1, 2)", ColumnTypes.TEXT, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id NUMERIC", ColumnTypes.NUMERIC, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id NUMERIC(1)", ColumnTypes.NUMERIC, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id NUMERIC(1, 2)", ColumnTypes.NUMERIC, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id NUM", ColumnTypes.NUMERIC, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id NUM(1)", ColumnTypes.NUMERIC, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id NUM(1, 2)", ColumnTypes.NUMERIC, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id INTEGER", ColumnTypes.INTEGER, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id INTEGER(1)", ColumnTypes.INTEGER, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id INTEGER(1, 2)", ColumnTypes.INTEGER, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id INT", ColumnTypes.INTEGER, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id INT(1)", ColumnTypes.INTEGER, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id INT(1, 2)", ColumnTypes.INTEGER, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id REAL", ColumnTypes.REAL, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id REAL(1)", ColumnTypes.REAL, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id REAL(1, 2)", ColumnTypes.REAL, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id BLOB", ColumnTypes.BLOB, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id BLOB(1)", ColumnTypes.BLOB, 1, Array.Empty<IColumnConstraint>() },
        new object[] { "Id BLOB(1, 2)", ColumnTypes.BLOB, 2, Array.Empty<IColumnConstraint>() },
        new object[] { "Id", ColumnTypes.BLOB, 0, Array.Empty<IColumnConstraint>() },
        new object[] { "Id integer primary key autoincrement", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { PrimaryKeyConstraint.Default() } },
        new object[] { "Id integer constraint pk primary key autoincrement", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { PrimaryKeyConstraint.Default("pk") } },
        new object[] { "Id integer constraint pk primary key desc on conflict rollback autoincrement", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { new PrimaryKeyConstraint("pk", Order.Desc, ConflictResolutions.Rollback, true) } },
        new object[] { "Id integer unique on conflict abort", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { new UniqueConstraint(String.Empty, ConflictResolutions.Abort) } },
        new object[] { "Id integer constraint id_u1 unique on conflict rollback", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { new UniqueConstraint("id_u1", ConflictResolutions.Rollback) } },
        new object[] { "Id integer constraint id_u1 unique", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { new UniqueConstraint("id_u1", ConflictResolutions.Default) } },
        new object[] { "Id int constraint pk primary key autoincrement not null", ColumnTypes.INTEGER, 0, new IColumnConstraint[] { PrimaryKeyConstraint.Default("pk"), NotNullConstraint.Default() } },
    };

    [Theory]
    [MemberData(nameof(ColumnDefData))]
    public void ColumnTest(string sql, ColumnTypes expectedType, int expectedModifierCount, IColumnConstraint[] expectedConstraints)
    {
        var tokens = Ddl.Tokenizer.Tokenize(sql);
        var result = Ddl.Column.TryParse(tokens);
        Assert.True(result.HasValue, result.ToString());
        var column = result.Value;
        Assert.Equal("Id", column.Name);

        Assert.NotNull(column.Type);
        var typeName = column.Type;
        Assert.Equal(expectedType, typeName.Type);

        Assert.Equal(expectedModifierCount, typeName.Modifier.Length);
        if (expectedModifierCount > 0)
        {
            Assert.Equal(1, typeName.Modifier.First());
        }

        if (expectedModifierCount > 1)
        {
            Assert.Equal(2, typeName.Modifier.Last());
        }

        Assert.Equal(expectedConstraints.Length, column.Constraints.Length);
        if (expectedConstraints.Length > 0)
        {
            Assert.Equivalent(expectedConstraints, column.Constraints);
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
        var tokens = Ddl.Tokenizer.Tokenize(ddl);
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
}
