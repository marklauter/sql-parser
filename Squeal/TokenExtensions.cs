namespace Squeal;

internal static class TokenExtensions
{
    public static bool IsStatementKeyword(this uint token) => (StatementTokens)token switch
    {
        StatementTokens.Create or
        StatementTokens.Select or
        StatementTokens.Alter or
        StatementTokens.Drop or
        StatementTokens.Insert or
        StatementTokens.Update or
        StatementTokens.Delete => true,
        _ => false,
    };
}
