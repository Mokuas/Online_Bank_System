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
            var existing = await maps.GetByUserIdAsync(message.UserId);
            if (existing is not null)
                return;

            var map = new UserCustomerMap
            {
                UserId = message.UserId,
                CustomerId = message.CustomerId,
                CreatedAt = DateTime.UtcNow
            };

            await maps.AddAsync(map);
            await maps.SaveChangesAsync();
        }
    }
}
