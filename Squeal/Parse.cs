using Superpower;
using Superpower.Model;

namespace Squeal;

internal static class Parse
{
    internal static readonly TextParser<string> AsString = input =>
    {
        var value = input.ToStringValue();
        return IsLiteral(value)
            ? Result.Value(value[1..^1], input, input.Skip(input.Length))
            : Result.Value(value, input, input.Skip(input.Length));
    };

    private static bool IsLiteral(string value) => value[0] == '\'' && value[^1] == '\'';
}
