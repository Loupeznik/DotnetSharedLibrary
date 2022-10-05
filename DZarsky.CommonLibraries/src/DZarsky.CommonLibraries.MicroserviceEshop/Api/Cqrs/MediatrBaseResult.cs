namespace DZarsky.CommonLibraries.MicroserviceEshop.Api.Cqrs
{
    public class MediatrBaseResult<TClass> where TClass : class
    {
        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public ResultStatus Status { get; set; }

        public TClass? Result { get; set; }

        public IList<string> Errors { get; set; } = new List<string>();

        public MediatrBaseResult(bool isSuccess, ResultStatus status, TClass? result = null, string? message = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Result = result;
            Status = status;
        }

        public MediatrBaseResult(ResultStatus status, IList<string> errors, string? message = null)
        {
            Message = message;
            Status = status;
            Errors = errors;
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
