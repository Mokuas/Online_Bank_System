using ProfilesService.Application.Common;
using ProfilesService.Application.Dtos.Customers;
using ProfilesService.Application.Repositories;
using ProfilesService.Application.Security;
using ProfilesService.Domain.Entities;
using ProfilesService.Domain.Enums;

namespace ProfilesService.Application.Services;

public sealed class CustomerService(ICustomerRepository customers, ICurrentUserService currentUser) : ICustomerService
{
    public async Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

        if (await customers.UserIdExistsAsync(userId.Value))
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.AlreadyExists, "Customer profile already exists for this user."));

        var customer = new Customer
        {
            UserId = userId.Value,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = request.DateOfBirth.ToUtcDateTime(),
            Address = request.Address.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            KycStatus = KycStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await customers.AddAsync(customer);
        await customers.SaveChangesAsync();

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> GetMeAsync()
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

        var customer = await customers.GetByUserIdReadAsync(userId.Value);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> GetByIdAsync(int id)
    {
        var customer = await customers.GetByIdReadAsync(id);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

        var customer = await customers.GetByIdAsync(id);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        if (!CanAccessCustomer(customer))
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Forbidden, "Forbidden."));

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.DateOfBirth = request.DateOfBirth.ToUtcDateTime();
        customer.Address = request.Address.Trim();
        customer.PhoneNumber = request.PhoneNumber.Trim();
        customer.Email = request.Email.Trim();

        await customers.SaveChangesAsync();

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<IReadOnlyList<CustomerResponse>>> GetByKycStatusAsync(KycStatus kycStatus)
    {
        if (!IsEmployeeOrAdmin())
            return Result<IReadOnlyList<CustomerResponse>>.Failure(
                new Error(ErrorCodes.Forbidden, "Forbidden."));

        var customerEntities = await customers.GetByKycStatusAsync(kycStatus);
        var result = customerEntities.Select(ToResponse).ToList().AsReadOnly();

        return Result<IReadOnlyList<CustomerResponse>>.Success(result);
    }

    public async Task<Result<CustomerResponse>> UpdateKycStatusAsync(int id, UpdateKycStatusRequest request)
    {
        if (!IsEmployeeOrAdmin())
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Forbidden, "Forbidden."));

        var customer = await customers.GetByIdAsync(id);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        customer.KycStatus = request.KycStatus;

        await customers.SaveChangesAsync();

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    private bool CanAccessCustomer(Customer customer)
    {
        var userId = currentUser.UserId;
        if (userId is not null && customer.UserId == userId.Value)
            return true;

        return IsEmployeeOrAdmin();
    }

    private bool IsEmployeeOrAdmin()
    {
        var role = currentUser.Role;
        return string.Equals(role, "Employee", StringComparison.OrdinalIgnoreCase)
               || string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
    }

    private static CustomerResponse ToResponse(Customer c)
        => new(
            Id: c.Id,
            UserId: c.UserId,
            FirstName: c.FirstName,
            LastName: c.LastName,
            DateOfBirth: DateOnly.FromDateTime(c.DateOfBirth),
            Address: c.Address,
            PhoneNumber: c.PhoneNumber,
            Email: c.Email,
            KycStatus: c.KycStatus,
            CreatedAt: c.CreatedAt
        );
}
