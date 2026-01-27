using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProfilesService.Application.Common;

namespace ProfilesService.Api.Errors
{
    public static class ErrorMappingExtensions
    {
        public static IActionResult ToActionResult(this Error error, ControllerBase controller)
        {
            var body = new ApiErrorResponse(error.Code, error.Message);

            return error.Code switch
            {
                ErrorCodes.Unauthorized => controller.Unauthorized(body),
                ErrorCodes.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, body),
                ErrorCodes.NotFound => controller.NotFound(body),
                ErrorCodes.AlreadyExists => controller.BadRequest(body),
                ErrorCodes.InvalidKycStatus => controller.BadRequest(body),
                _ => controller.BadRequest(body)
            };
        }
    }
}
