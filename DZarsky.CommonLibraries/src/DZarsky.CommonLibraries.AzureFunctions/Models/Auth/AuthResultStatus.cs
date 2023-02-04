using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DZarsky.CommonLibraries.AzureFunctions.Models.Auth
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AuthResultStatus
    {
        Success,
        InvalidLoginOrPassword,
        UserInactive,
        Error
    }
}
