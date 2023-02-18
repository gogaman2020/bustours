using Infrastructure.Security.Models;

namespace Infrastructure.Security.SecurityTokens
{
    /// <summary>
    /// Куки
    /// </summary>
    public interface ISecurityTokensProvider
    {
        string SecurityTokenName { get; }
        
        string SecurityToken { get; }
        
        string SecurityTokenV2Name { get; }
        
        string SecurityTokenV2 { get; }
        
        UserInfo UserInfo { get; }
        
        public int CacheId { get; }
    }
}