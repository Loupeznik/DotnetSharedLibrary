namespace DZarsky.CommonLibraries.AzureFunctions.Configuration;

internal sealed class ZitadelConfiguration
{
    public string? Authority { get; init; }

    public string? ClientId { get; init; }

    public string? ClientSecret { get; init; }

    public static string? IntrospectionEndpoint => "/oauth/v2/introspect";

    public static string? UserInfoEndpoint => $"/oidc/v1/userinfo";
}
