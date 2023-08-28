namespace DZarsky.CommonLibraries.AzureFunctions.Models.Auth;

public enum AuthType
{
    // Auth is handled by the function
    Standalone,
    // Auth is handled via OIDC with AzureAD
    AzureAd,
    // Auth is handled via another identity function
    IdentityServer,
    // Auth is handled via OIDC with a Zitadel instance
    Zitadel
}
