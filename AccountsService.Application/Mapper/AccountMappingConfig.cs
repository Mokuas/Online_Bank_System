using AccountsService.Application.Dtos.Accounts;
using AccountsService.Domain.Entities;
using Mapster;

namespace AccountsService.Application.Mapper
{
    public sealed class AccountMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Account, AccountResponse>();
        }
    }
}
