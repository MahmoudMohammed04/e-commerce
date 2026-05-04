using HomeCare.Services.Result;
using Microsoft.AspNetCore.Mvc;

namespace HomeCare.Extentions
{
    public static class ServiceResultExtention
    {
        public static IActionResult ErrorToActionResult<T>(this IServiceResult<T> result)
        {

            return result.ErrorType switch
            {
                ErrorTypeEnum.BAD_REQUEST => new BadRequestObjectResult(result.ErrorMessage),
                ErrorTypeEnum.NOT_FOUND => new NotFoundObjectResult(result.ErrorMessage),
                ErrorTypeEnum.UNAUTHORIZED => new UnauthorizedObjectResult(result.ErrorMessage),
                ErrorTypeEnum.CONFLICT => new ConflictObjectResult(result.ErrorMessage),
                ErrorTypeEnum.SERVER_ERROR => new ObjectResult(result.ErrorMessage) { StatusCode = 500 },
                _ => new ObjectResult(result.ErrorMessage) { StatusCode = 500 }
            };
        }

        public static ActionResult<K> ErrorToActionResult<T, K>(this IServiceResult<T> result)
        {

            return result.ErrorType switch
            {
                ErrorTypeEnum.BAD_REQUEST => new BadRequestObjectResult(result.ErrorMessage),
                ErrorTypeEnum.NOT_FOUND => new NotFoundObjectResult(result.ErrorMessage),
                ErrorTypeEnum.UNAUTHORIZED => new UnauthorizedObjectResult(result.ErrorMessage),
                ErrorTypeEnum.CONFLICT => new ConflictObjectResult(result.ErrorMessage),
                ErrorTypeEnum.SERVER_ERROR => new ObjectResult(result.ErrorMessage) { StatusCode = 500 },
                _ => new ObjectResult(result.ErrorMessage) { StatusCode = 500 }
            };
        }

        public static ActionResult<T> ErrorToGenericActionResult<T>(this IServiceResult<T> result)
        {

            return result.ErrorType switch
            {
                ErrorTypeEnum.BAD_REQUEST => new BadRequestObjectResult(result.ErrorMessage),
                ErrorTypeEnum.NOT_FOUND => new NotFoundObjectResult(result.ErrorMessage),
                ErrorTypeEnum.UNAUTHORIZED => new UnauthorizedObjectResult(result.ErrorMessage),
                ErrorTypeEnum.CONFLICT => new ConflictObjectResult(result.ErrorMessage),
                ErrorTypeEnum.SERVER_ERROR => new ObjectResult(result.ErrorMessage) { StatusCode = 500 },
                _ => new ObjectResult(result.ErrorMessage) { StatusCode = 500 }
            };
        }
    }
}
