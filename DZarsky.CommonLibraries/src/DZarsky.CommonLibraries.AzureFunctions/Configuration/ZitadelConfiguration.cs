namespace DZarsky.CommonLibraries.AzureFunctions.Configuration;

public sealed class ZitadelConfiguration
{
    public string? Authority { get; set; }

    public string? JwtProfile { get; set; }

    public IList<string> DebugRoles { get; set; } = new List<string>();
}
