using AccountsService.Api.Errors;
using AccountsService.Application.Dtos.Accounts;
using AccountsService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountsService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AccountsController(IAccountService accountService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Open([FromBody] OpenAccountRequest request)
    {
        var result = await accountService.OpenAsync(request);

        if (!result.IsSuccess || result.Value is null)
            return result.Error!.ToActionResult(this);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await accountService.GetMeAsync();

        if (!result.IsSuccess || result.Value is null)
            return result.Error!.ToActionResult(this);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await accountService.GetByIdAsync(id);

        if (!result.IsSuccess || result.Value is null)
            return result.Error!.ToActionResult(this);

        return Ok(result.Value);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetByCustomerId([FromQuery] int customerId)
    {
        var result = await accountService.GetByCustomerIdAsync(customerId);

        if (!result.IsSuccess || result.Value is null)
            return result.Error!.ToActionResult(this);

        return Ok(result.Value);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] int id,
        [FromBody] ChangeAccountStatusRequest request)
    {
        var result = await accountService.ChangeStatusAsync(id, request);

        if (!result.IsSuccess || result.Value is null)
            return result.Error!.ToActionResult(this);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("{id:int}/balance")]
    public async Task<IActionResult> GetBalance([FromRoute] int id)
    {
        var result = await accountService.GetBalanceAsync(id);

        if (!result.IsSuccess || result.Value is null)
            return result.Error!.ToActionResult(this);

        return Ok(result.Value);
    }
}
