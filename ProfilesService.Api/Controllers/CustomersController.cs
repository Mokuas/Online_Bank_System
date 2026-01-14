using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProfilesService.Application.Dtos.Customers;
using ProfilesService.Application.Services;
using ProfilesService.Application.Common;
using ProfilesService.Api.Errors;
using ProfilesService.Domain.Enums;

namespace ProfilesService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class CustomersController(ICustomerService customerService) : ControllerBase
    {
        private readonly ICustomerService _customerService = customerService;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var result = await _customerService.CreateAsync(request);

            if (!result.IsSuccess || result.Value is null)
                return MapError(result.Error!);

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var result = await _customerService.GetMeAsync();

            if (!result.IsSuccess || result.Value is null)
                return MapError(result.Error!);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _customerService.GetByIdAsync(id);

            if (!result.IsSuccess || result.Value is null)
                return MapError(result.Error!);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
        {
            var result = await _customerService.UpdateAsync(id, request);

            if (!result.IsSuccess || result.Value is null)
                return MapError(result.Error!);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetByKycStatus([FromQuery] KycStatus kycStatus)
        {
            var result = await _customerService.GetByKycStatusAsync(kycStatus);

            if (!result.IsSuccess || result.Value is null)
                return MapError(result.Error!);

            return Ok(result.Value);
        }

        [Authorize(Roles = "Employee,Admin")]
        [HttpPatch("{id:int}/kyc-status")]
        public async Task<IActionResult> UpdateKycStatus([FromRoute] int id, [FromBody] UpdateKycStatusRequest request)
        {
            var result = await _customerService.UpdateKycStatusAsync(id, request);

            if (!result.IsSuccess || result.Value is null)
                return MapError(result.Error!);

            return Ok(result.Value);
        }

        private IActionResult MapError(Error error)
        {
            var body = new ApiErrorResponse(error.Code, error.Message);

            return error.Code switch
            {
                ErrorCodes.Unauthorized => Unauthorized(body),
                ErrorCodes.Forbidden => StatusCode(StatusCodes.Status403Forbidden, body),
                ErrorCodes.NotFound => NotFound(body),
                ErrorCodes.AlreadyExists => BadRequest(body),
                ErrorCodes.InvalidKycStatus => BadRequest(body),
                _ => BadRequest(body)
            };
        }
    }
}
