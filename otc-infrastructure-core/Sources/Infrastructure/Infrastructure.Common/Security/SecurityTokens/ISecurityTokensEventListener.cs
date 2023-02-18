namespace Infrastructure.Security.SecurityTokens
{
    public interface ISecurityTokensEventListener
    {
        public void TokensChanged(ISecurityTokensProvider provider);
    }
}