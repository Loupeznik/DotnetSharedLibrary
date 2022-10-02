namespace DZarsky.CommonLibraries.MicroserviceEshop.Api.Requests
{
    public interface IClientSessionContextRequest
    {
        public Guid? SessionID { get; set; }

        public Guid? UserID { get; set; }
    }
}
