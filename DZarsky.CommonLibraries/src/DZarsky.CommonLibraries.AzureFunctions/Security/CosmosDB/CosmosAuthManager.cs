using DZarsky.CommonLibraries.AzureFunctions.Configuration;
using DZarsky.CommonLibraries.AzureFunctions.Models.Auth;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Text;

using CosmosUser = DZarsky.CommonLibraries.AzureFunctions.Models.Users.User;

namespace DZarsky.CommonLibraries.AzureFunctions.Security.CosmosDB;

/// <summary>
/// Handles custom authentication via CosmosDB
/// </summary>
public sealed class CosmosAuthManager : IAuthManager
{
    private readonly CosmosClient _db;
    private readonly PasswordUtils _passwordUtils;
    private readonly CosmosConfiguration _configuration;

    public CosmosAuthManager(CosmosClient db, PasswordUtils utils, CosmosConfiguration configuration)
    {
        _db = db;
        _passwordUtils = utils;
        _configuration = configuration;
    }

    /// <summary>
    /// Validates a token
    /// </summary>
    /// <param name="token">The contents of Authorization header</param>
    /// <returns></returns>
    public async Task<AuthResult> ValidateToken(string token)
    {
        var credentials = ParseToken(token);

        if (credentials is null)
        {
            return new AuthResult(AuthResultStatus.InvalidLoginOrPassword);
        }

        if (string.IsNullOrWhiteSpace(credentials.Login) || string.IsNullOrWhiteSpace(credentials.Password))
        {
            return new AuthResult(AuthResultStatus.InvalidLoginOrPassword);
        }

        var container = _db.GetContainer(_configuration.DatabaseID, _configuration.UsersContainerID);

        var query = container
            .GetItemLinqQueryable<CosmosUser>()
            .Where(x => x.Login == credentials.Login)
            .ToFeedIterator();

        var user = (await query.ReadNextAsync()).FirstOrDefault();

        if (user == null || !_passwordUtils.ValidatePassword(credentials.Password, user.Password!))
        {
            return new AuthResult(AuthResultStatus.InvalidLoginOrPassword);
        }

        if (!user.IsActive)
        {
            return new AuthResult(AuthResultStatus.UserInactive);
        }

        return new AuthResult(AuthResultStatus.Success, user.Id);
    }

    private static CosmosUser? ParseToken(string header)
    {
        if (!string.IsNullOrEmpty(header) && header.StartsWith("Basic"))
        {
            var base64Credentials = header["Basic ".Length..].Trim();

            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credentials = encoding.GetString(Convert.FromBase64String(base64Credentials));

            int seperatorIndex = credentials.IndexOf(':');

            return new CosmosUser
            {
                Login = credentials[..seperatorIndex],
                Password = credentials[(seperatorIndex + 1)..]
            };
        }
        else
        {
            return null;
        }
    }
}
