namespace DZarsky.CommonLibraries.MicroserviceEshop.Api.Cqrs
{
    public class MediatrBaseResult<TClass> where TClass : class
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public ResultStatus Status { get; set; }

        public TClass? Result { get; set; }

        public MediatrBaseResult(bool isSuccess, ResultStatus status, TClass? result = null, string? message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Result = result;
            Status = status;
        }
    }

    public enum ResultStatus
    {
        Success,
        EntityNotFound,
        Unauthorized,
        Conflict,
        BadRequest
    }
}
