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
    public sealed class AccountService(
    IAccountRepository accounts,
    IAccountQueries accountQueries,
    ICurrentUserService currentUser,
    ICustomerIdResolver customerIdResolver,
    IIntegrationEventPublisher publisher,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IAccountService
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

            await using var tx = await unitOfWork.BeginTransactionAsync(ct);

            await accounts.AddAsync(entity, ct);

            // Save account first to get entity.Id (identity)
            await unitOfWork.SaveChangesAsync(ct);

            await publisher.PublishAsync(
                new AccountCreated(
                    AccountId: entity.Id,
                    CustomerId: entity.CustomerId,
                    AccountNumber: entity.AccountNumber,
                    Currency: entity.Currency),
                ct);

            // Persist outbox record in same transaction
            await unitOfWork.SaveChangesAsync(ct);

            // Commit transaction
            await tx.CommitAsync(ct);

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

            var mapped = await accountQueries.GetByCustomerIdAsync(customerId.Value, ct);
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

            // Now return projected DTO (no tracking, DTO projection)
            var dto = await accountQueries.GetByIdAsync(id, ct);
            if (dto is null)
                return Result<AccountResponse>.Failure(new Error(ErrorCodes.NotFound, "Account not found."));

            return Result<AccountResponse>.Success(dto);
        }

        public async Task<Result<IReadOnlyList<AccountResponse>>> GetByCustomerIdAsync(int customerId, CancellationToken ct)
        {
            var role = currentUser.Role;
            if (role is not "Employee" and not "Admin")
                return Result<IReadOnlyList<AccountResponse>>.Failure(new Error(ErrorCodes.Forbidden, "Forbidden."));

            var mapped = await accountQueries.GetByCustomerIdAsync(customerId, ct);
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

            await using var tx = await unitOfWork.BeginTransactionAsync(ct);

            account.Status = request.Status;

            // Persist status change
            await unitOfWork.SaveChangesAsync(ct);

            // Enqueue event to Outbox
            await publisher.PublishAsync(
                new AccountStatusChanged(
                    AccountId: account.Id,
                    Status: account.Status),
                ct);

            // Persist outbox record
            await unitOfWork.SaveChangesAsync(ct);

            await tx.CommitAsync(ct);

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
