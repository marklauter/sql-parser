using Superpower;
using Superpower.Model;

namespace Squeal;

internal static class Parse
{
    internal static readonly TextParser<string> AsString = input =>
        Result.Value(input.ToString(), input, input.Skip(input.Length));
}
