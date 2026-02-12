using AccountsService.Application.IntegrationEvents;
using AccountsService.Application.Messaging;
using AccountsService.Application.Repositories;
using AccountsService.Domain.Entities;

namespace AccountsService.Application.IntegrationHandlers
{
    public sealed class CustomerProfileCreatedHandler(IUserCustomerMapRepository maps) : IIntegrationEventHandler<CustomerProfileCreated>
    {
        public async Task HandleAsync(CustomerProfileCreated message, CancellationToken ct)
        {
            var existingCustomerId = await maps.GetCustomerIdByUserIdAsync(message.UserId, ct);
            if (existingCustomerId is not null)
                return;

            await maps.AddAsync(message.UserId, message.CustomerId, ct);
            await maps.SaveChangesAsync(ct);
        }
    }
}
