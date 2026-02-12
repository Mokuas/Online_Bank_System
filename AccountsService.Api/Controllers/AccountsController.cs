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
    public async Task<IActionResult> Open([FromBody] OpenAccountRequest request, CancellationToken ct)
    {
        var result = await accountService.OpenAsync(request, ct);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct)
    {
        var result = await accountService.GetMeAsync(ct);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
    {
        var result = await accountService.GetByIdAsync(id, ct);

        return result.ToActionResult(this);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetByCustomerId([FromQuery] int customerId, CancellationToken ct)
    {
        var result = await accountService.GetByCustomerIdAsync(customerId, ct);

        return result.ToActionResult(this);
    }

    [Authorize(Roles = "Employee,Admin")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> ChangeStatus(
        [FromRoute] int id,
        [FromBody] ChangeAccountStatusRequest request, CancellationToken ct)
    {
        var result = await accountService.ChangeStatusAsync(id, request, ct);

        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("{id:int}/balance")]
    public async Task<IActionResult> GetBalance([FromRoute] int id, CancellationToken ct)
    {
        var result = await accountService.GetBalanceAsync(id, ct);

        return result.ToActionResult(this);
    }
}
