using Abp.Application.Services;
using KTGKMD.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace KTGKMD.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
