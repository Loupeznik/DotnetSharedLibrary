using Microsoft.AspNetCore.Http;

namespace DZarsky.CommonLibraries.MicroserviceEshop.Api.Cqrs
{
    public static class ResponseStatusCodeHelper
    {
        public static int DetermineStatusCode(ResultStatus status)
        {
            return status switch
            {
                ResultStatus.Success => StatusCodes.Status200OK,
                ResultStatus.EntityNotFound => StatusCodes.Status404NotFound,
                ResultStatus.Unauthorized => StatusCodes.Status401Unauthorized,
                ResultStatus.Conflict => StatusCodes.Status409Conflict,
                ResultStatus.BadRequest => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status200OK,
            };
        }
    }
}
