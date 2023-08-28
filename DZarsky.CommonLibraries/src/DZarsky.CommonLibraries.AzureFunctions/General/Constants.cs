namespace DZarsky.CommonLibraries.AzureFunctions.General
{
    public static class ApiConstants
    {
        public const string BasicAuthSchemeID = "basic_auth";
        public const string ApiKeyAuthSchemeID = "api_key";

        public const string JsonContentType = "application/json";
    }

    public static class HttpClients
    {
        public const string ZitadelClient = nameof(ZitadelClient);
    }
}
