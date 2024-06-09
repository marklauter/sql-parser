using Lexi;

namespace Squeal;

internal readonly ref struct ParseResult<T>(
    T? result,
    MatchResult matchResult)
{
    public readonly T? Result = result;
    public readonly MatchResult MatchResult = matchResult;
}
