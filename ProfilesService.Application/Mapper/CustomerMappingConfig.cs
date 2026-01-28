using Mapster;
using ProfilesService.Domain.Entities;
using ProfilesService.Application.Dtos.Customers;

namespace ProfilesService.Application.Mapper
{
    public sealed class CustomerMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Customer, CustomerResponse>()
                .Map(dest => dest.DateOfBirth,
                     src => DateOnly.FromDateTime(src.DateOfBirth));
        }
    }
}
