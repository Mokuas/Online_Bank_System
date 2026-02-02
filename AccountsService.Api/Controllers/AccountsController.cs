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

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var result = await accountService.GetMeAsync();

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await accountService.GetByIdAsync(id);

        return result.ToActionResult(this);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetByCustomerId([FromQuery] int customerId)
    {
        var result = await accountService.GetByCustomerIdAsync(customerId);

        return result.ToActionResult(this);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] int id,
        [FromBody] ChangeAccountStatusRequest request)
    {
        var result = await accountService.ChangeStatusAsync(id, request);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("{id:int}/balance")]
    public async Task<IActionResult> GetBalance([FromRoute] int id)
    {
        var result = await accountService.GetBalanceAsync(id);

        return result.ToActionResult(this);
    }
}
