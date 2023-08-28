using DZarsky.CommonLibraries.AzureFunctions.Models.Auth;

namespace DZarsky.CommonLibraries.AzureFunctions.Security;

public interface IAuthManager
{
    /// <summary>
    /// Validates a token
    /// </summary>
    /// <param name="token">The authorization token</param>
    /// <returns></returns>
    Task<AuthResult> ValidateToken(string token);
}
