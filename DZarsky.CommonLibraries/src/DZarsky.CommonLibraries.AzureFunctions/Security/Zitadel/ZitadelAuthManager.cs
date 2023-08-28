using DZarsky.CommonLibraries.AzureFunctions.Configuration;
using DZarsky.CommonLibraries.AzureFunctions.General;
using DZarsky.CommonLibraries.AzureFunctions.Models.Auth;
using Newtonsoft.Json.Linq;

namespace DZarsky.CommonLibraries.AzureFunctions.Security.Zitadel;

/// <summary>
/// Handles authentication via OIDC with Zitadel
/// </summary>
public sealed class ZitadelAuthManager : IAuthManager
{
    private readonly HttpClient _client;

    public ZitadelAuthManager(IHttpClientFactory factory) => _client = factory.CreateClient(HttpClients.ZitadelClient);

    /// <summary>
    /// Validates a token
    /// </summary>
    /// <param name="token">The access token issued by Zitadel</param>
    /// <returns></returns>
    public async Task<AuthResult> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthResult(AuthResultStatus.Error);
        }

        var request = await _client.PostAsync(ZitadelConfiguration.IntrospectionEndpoint, new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("token", token.Replace("Bearer ", ""))
        }));

        if (!request.IsSuccessStatusCode)
        {
            return new AuthResult(AuthResultStatus.Error);
        }

        var result = JObject.Parse(await request.Content.ReadAsStringAsync());

        if (result["active"]?.Value<bool>() != true)
        {
            return new AuthResult(AuthResultStatus.Error);
        }

        return new AuthResult(AuthResultStatus.Success, result["sub"]?.Value<string>());
    }
}
