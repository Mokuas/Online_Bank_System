using AccountsService.Application.Common;
using AccountsService.Application.Dtos.Accounts;
using AccountsService.Application.IntegrationEvents;
using AccountsService.Application.Messaging;
using AccountsService.Application.Repositories;
using AccountsService.Application.Security;
using AccountsService.Domain.Entities;
using AccountsService.Domain.Enums;
using MapsterMapper;
using System.Security.Cryptography;

namespace AccountsService.Application.Services
{
    public sealed class AccountService(IAccountRepository accounts, ICurrentUserService currentUser, ICustomerIdResolver customerIdResolver, IEventBus eventBus,IMapper mapper) : IAccountService
    {
        public async Task<Result<AccountResponse>> OpenAsync(OpenAccountRequest request, CancellationToken  ct)
        {
            var role = currentUser.Role;

            if (string.IsNullOrWhiteSpace(role))
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

            if (role is not "Employee" and not "Admin")
            {
                var myCustomerId = await customerIdResolver.ResolveAsync();
                if (myCustomerId is null)
                {
                    return Result<AccountResponse>.Failure(
                        new Error(ErrorCodes.Forbidden, "Customer mapping not found. Please complete your profile/KYC first."));
                }

                if (myCustomerId.Value != request.CustomerId)
                {
                    return Result<AccountResponse>.Failure(
                        new Error(ErrorCodes.Forbidden, "You cannot open an account for another customer."));
                }
            }

            var accountNumber = await GenerateUniqueAccountNumberAsync(ct);

            var entity = new Account
            {
                CustomerId = request.CustomerId,
                AccountNumber = accountNumber,
                Type = request.Type,
                Currency = request.Currency,
                Balance = 0m,
                Status = AccountStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            await accounts.AddAsync(entity, ct);
            await accounts.SaveChangesAsync(ct);

            await eventBus.PublishAsync(
                 RoutingKeys.AccountCreated,
                 new AccountCreated(
                    AccountId: entity.Id,
                    CustomerId: entity.CustomerId,
                    AccountNumber: entity.AccountNumber,
                    Currency: entity.Currency));

            var response = mapper.Map<AccountResponse>(entity);
            return Result<AccountResponse>.Success(response);
        }

        public async Task<Result<IReadOnlyList<AccountResponse>>> GetMeAsync(CancellationToken ct)
        {
            var customerId = await customerIdResolver.ResolveAsync();
            if (customerId is null)
            {
                return Result<IReadOnlyList<AccountResponse>>.Failure(
                    new Error(ErrorCodes.Forbidden, "Customer mapping not found. Please complete your profile first."));
            }

            var items = await accounts.GetByCustomerIdAsync(customerId.Value, ct);
            var mapped = items.Select(a => mapper.Map<AccountResponse>(a)).ToList().AsReadOnly();

            return Result<IReadOnlyList<AccountResponse>>.Success(mapped);
        }

        public async Task<Result<AccountResponse>> GetByIdAsync(int id, CancellationToken ct)
        {
            var account = await accounts.GetByIdReadAsync(id, ct);
            if (account is null)
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.NotFound, "Account not found."));

            var access = await CheckReadAccessAsync(account, ct);

            if (!access.IsSuccess)
                return Result<AccountResponse>.Failure(access.Error!);

            return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
        }

        public async Task<Result<IReadOnlyList<AccountResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken ct)
        {
            var role = currentUser.Role;
            if (role is not "Employee" and not "Admin")
                return Result<IReadOnlyList<AccountResponse>>.Failure(
                    new Error(ErrorCodes.Forbidden, "Forbidden."));

            var items = await accounts.GetByCustomerIdAsync(customerId, ct);
            var mapped = items.Select(a => mapper.Map<AccountResponse>(a)).ToList().AsReadOnly();

            return Result<IReadOnlyList<AccountResponse>>.Success(mapped);
        }

        public async Task<Result<AccountResponse>> ChangeStatusAsync(int id, ChangeAccountStatusRequest request, CancellationToken ct)
        {
            // Employee/Admin only (locking/closing is staff action)
            var role = currentUser.Role;
            if (role is not "Employee" and not "Admin")
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.Forbidden, "Forbidden."));

            var account = await accounts.GetByIdAsync(id, ct);
            if (account is null)
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.NotFound, "Account not found."));

            if (account.Status == AccountStatus.Closed)
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.BadRequest, "Closed accounts cannot be modified."));

            if (request.Status == AccountStatus.Active && account.Status == AccountStatus.Active)
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.BadRequest, "Account is already Active."));

            if (request.Status == AccountStatus.Locked && account.Status == AccountStatus.Locked)
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.BadRequest, "Account is already Locked."));

            account.Status = request.Status;

            await accounts.SaveChangesAsync(ct);

            await eventBus.PublishAsync(
                RoutingKeys.AccountStatusChanged,
                new AccountStatusChanged(
                    AccountId: account.Id,
                    Status: account.Status));

            return Result<AccountResponse>.Success(mapper.Map<AccountResponse>(account));
        }

        public async Task<Result<BalanceResponse>> GetBalanceAsync(int id, CancellationToken ct)
        {
            var account = await accounts.GetByIdReadAsync(id, ct);
            if (account is null)
                return Result<BalanceResponse>.Failure(new Error(ErrorCodes.NotFound, "Account not found."));

            var access = await CheckReadAccessAsync(account, ct);
            if (!access.IsSuccess)
                return Result<BalanceResponse>.Failure(access.Error!);

            return Result<BalanceResponse>.Success(new BalanceResponse(account.Id, account.Balance, account.Currency));
        }

        private async Task<Result> CheckReadAccessAsync(Account account, CancellationToken ct)
        {
            var role = currentUser.Role;

            if (role is "Employee" or "Admin")
                return Result.Success();

            var myCustomerId = await customerIdResolver.ResolveAsync();
            if (myCustomerId is null)
                return Result.Failure(new Error(ErrorCodes.Forbidden, "Customer mapping not found."));

            if (myCustomerId.Value != account.CustomerId)
                return Result.Failure(new Error(ErrorCodes.Forbidden, "You do not have access to this account."));

            return Result.Success();
        }

        private async Task<string> GenerateUniqueAccountNumberAsync(CancellationToken ct)
        {
            // 16-digit numeric account number (simple + unique check)
            for (var attempt = 0; attempt < 20; attempt++)
            {
                var digits = RandomNumberGenerator.GetInt32(0, int.MaxValue).ToString("D10")
                           + RandomNumberGenerator.GetInt32(0, int.MaxValue).ToString("D10");

                var accountNumber = digits[..16];

                if (!await accounts.AccountNumberExistsAsync(accountNumber, ct))
                    return accountNumber;
            }

            throw new InvalidOperationException("Failed to generate a unique account number.");
        }
    }
}
