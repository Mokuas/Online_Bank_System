using ProfilesService.Application.Common;
using ProfilesService.Application.Dtos.Customers;
using ProfilesService.Application.Repositories;
using ProfilesService.Application.Security;
using ProfilesService.Domain.Entities;
using ProfilesService.Domain.Enums;

namespace ProfilesService.Application.Services;

public sealed class CustomerService(ICustomerRepository customers, ICurrentUserService currentUser) : ICustomerService
{

    private readonly ICustomerRepository _customers = customers;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<Result<CustomerResponse>> CreateAsync(CreateCustomerRequest request)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

        if (await _customers.UserIdExistsAsync(userId.Value))
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.AlreadyExists, "Customer profile already exists for this user."));

        var customer = new Customer
        {
            UserId = userId.Value,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            DateOfBirth = ToDateTime(request.DateOfBirth),
            Address = request.Address.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            KycStatus = KycStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _customers.AddAsync(customer);
        await _customers.SaveChangesAsync();

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> GetMeAsync()
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

        var customer = await _customers.GetByUserIdAsync(userId.Value);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> GetByIdAsync(int id)
    {
        var customer = await _customers.GetByIdAsync(id);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<CustomerResponse>> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Unauthorized, "Unauthorized."));

        var customer = await _customers.GetByIdAsync(id);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        if (!CanAccessCustomer(customer))
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Forbidden, "Forbidden."));

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.DateOfBirth = ToDateTime(request.DateOfBirth);
        customer.Address = request.Address.Trim();
        customer.PhoneNumber = request.PhoneNumber.Trim();
        customer.Email = request.Email.Trim();

        await _customers.SaveChangesAsync();

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    public async Task<Result<IReadOnlyList<CustomerResponse>>> GetByKycStatusAsync(KycStatus kycStatus)
    {
        if (!IsEmployeeOrAdmin())
            return Result<IReadOnlyList<CustomerResponse>>.Failure(
                new Error(ErrorCodes.Forbidden, "Forbidden."));

        var customers = await _customers.GetByKycStatusAsync(kycStatus);
        var result = customers.Select(ToResponse).ToList().AsReadOnly();

        return Result<IReadOnlyList<CustomerResponse>>.Success(result);
    }

    public async Task<Result<CustomerResponse>> UpdateKycStatusAsync(int id, UpdateKycStatusRequest request)
    {
        if (!IsEmployeeOrAdmin())
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.Forbidden, "Forbidden."));

        var customer = await _customers.GetByIdAsync(id);
        if (customer is null)
            return Result<CustomerResponse>.Failure(new Error(ErrorCodes.NotFound, "Customer profile not found."));

        customer.KycStatus = request.KycStatus;

        await _customers.SaveChangesAsync();

        return Result<CustomerResponse>.Success(ToResponse(customer));
    }

    private bool CanAccessCustomer(Customer customer)
    {
        var userId = _currentUser.UserId;
        if (userId is not null && customer.UserId == userId.Value)
            return true;

        return IsEmployeeOrAdmin();
    }

    private bool IsEmployeeOrAdmin()
    {
        var role = _currentUser.Role;
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

    private static DateTime ToDateTime(DateOnly date)
        => date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
}
