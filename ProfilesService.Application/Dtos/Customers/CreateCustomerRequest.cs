namespace ProfilesService.Application.Dtos.Customers
{
    public sealed record CreateCustomerRequest(
    string FirstName,
    string LastName,
    DateOnly DateOfBirth,
    string Address,
    string PhoneNumber,
    string Email);
}