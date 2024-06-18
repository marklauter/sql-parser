<div>
<img src="https://github.com/marklauter/sql-parser/blob/main/images/squeal.svg" title="squeal-logo" alt="squeal-logo" height="128" />
<img src="https://github.com/devicons/devicon/blob/master/icons/sqlite/sqlite-original.svg" title="sqlite-logo" alt="sqlite-logo" height="128" />
<img src="https://github.com/devicons/devicon/blob/master/icons/csharp/csharp-original.svg" title="csharp-logo" alt="csharp-logo" height="128" />


# Squeal
A C# SQLite SQL and DDL parser built with [Superpower](https://github.com/datalust/superpower) for the [Code Crafters SQLite challenge](https://app.codecrafters.io/courses/sqlite/introduction).

## Dev Log
- 2024 JUN 10 - Create table statement nearly complete. The Squeal library project doesn't build. You can unload it. The working code is in the unit tests.
- 2024 JUN 11 - The next step is the simplest select statement. That should be enough to complete the next stage of the Code Crafters challenge.
- 2024 JUN 11 - Switched to the Superpower tokenizer. This saves the hassle of manually skipping whitespace. The library builds again. Tests are unorganized.
- 2024 JUN 12 - Library builds and tests are organized. Finished simple column DDL parser. The next step is to add `column-constraint` to the column parser. I'm almost finished with a simple version of [create-table-stmt](https://www.sqlite.org/syntax/create-table-stmt.html).
- 2024 JUN 16 - Simple create table parse complete. Check, default, generated, and foreign key constraints, and as [seelect], might not be required for the Code Crafters challenge, so I'm deferring their implementation. Same with table constraints and table options.
- 2024 JUN 17 - Minimum viable select statement support. `select *`, `select count(*)`, and `select col` all work.
</div>
