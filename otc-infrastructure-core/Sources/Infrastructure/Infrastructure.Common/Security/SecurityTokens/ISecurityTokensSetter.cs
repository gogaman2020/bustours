namespace Infrastructure.Security.SecurityTokens
{
    public interface ISecurityTokensSetter : ISecurityTokensProvider
    {
        void SetTokens(string token, string tokenv2);
    }
}