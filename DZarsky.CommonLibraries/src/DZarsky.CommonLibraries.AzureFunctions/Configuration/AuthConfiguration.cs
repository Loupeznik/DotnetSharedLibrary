namespace DZarsky.CommonLibraries.AzureFunctions.Configuration
{
    public sealed class AuthConfiguration
    {
        public string ArgonSecret { get; set; } = string.Empty;

        public int HashLength { get; set; } = 20;

        public int SaltBytes { get; set; } = 16;
    }
}
