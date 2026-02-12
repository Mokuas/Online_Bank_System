using AccountsService.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace AccountsService.Api.Errors
{
    public static class ResultMappingExtensions
    {
        public static IActionResult ToActionResult<T>(
            this Result<T> result,
            ControllerBase controller)
        {
            if (!result.IsSuccess || result.Value is null)
                return (result.Error ?? new Error(ErrorCodes.BadRequest, "Unknown error."))
                    .ToActionResult(controller);

            return controller.Ok(result.Value);
        }

        public static IActionResult ToActionResult<T>(
            this Result<T> result,
            ControllerBase controller,
            Func<T, IActionResult> onSuccess)
        {
            if (!result.IsSuccess || result.Value is null)
                return (result.Error ?? new Error(ErrorCodes.BadRequest, "Unknown error."))
                    .ToActionResult(controller);

            return onSuccess(result.Value);
        }
    }
}
