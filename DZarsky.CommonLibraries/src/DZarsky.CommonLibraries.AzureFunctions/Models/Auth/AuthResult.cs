namespace DZarsky.CommonLibraries.AzureFunctions.Models.Auth
{
    public class AuthResult
    {
        public AuthResultStatus Status { get; set; }

        public string? UserID { get; set; }

        public AuthResult(AuthResultStatus status, string? userID = null)
        {
            Status = status;
            UserID = userID;
        }
    }
}
