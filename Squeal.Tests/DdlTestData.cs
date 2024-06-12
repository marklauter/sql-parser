using System.Collections;

namespace Squeal.Tests;

public sealed class DdlTestData
    : IEnumerable<object?[]>
{
    public static readonly string CreateTableDdl = File.ReadAllText("create-table-apples.sql");
    public static readonly string CreateTableSimpleColumnsDdl = File.ReadAllText("create-table-apples-simple-columns.sql");
    public static readonly string CreateTempTableDdl = File.ReadAllText("create-temp-table-apples.sql");
    public static readonly string CreateSchemaTableDdl = File.ReadAllText("create-table-trees-apples.sql");
    public static readonly string CreateTableIfNotExistsDdl = File.ReadAllText("create-table-ifnotexists-apples.sql");

    private readonly List<object?[]> data =
    [
        [ CreateTableDdl, false, false, "apples", null ],
        [ CreateTableSimpleColumnsDdl, false, false, "apples", null ],
        [ CreateTempTableDdl, true, false, "apples", null ],
        [ CreateSchemaTableDdl, false, false, "apples", "trees" ],
        [ CreateTableIfNotExistsDdl, false, true, "apples", null ]
    ];

    public IEnumerator<object?[]> GetEnumerator() => data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
