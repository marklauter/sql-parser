﻿using System.Diagnostics.CodeAnalysis;

namespace Squeal.CreateStatement;

[SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "doesn't matter")]
public enum ColumnTypes
{
    BLOB,
    INTEGER,
    NUMERIC,
    REAL,
    TEXT
}
