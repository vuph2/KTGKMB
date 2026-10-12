using Abp.Application.Services;
using KTGKMD.Sessions.Dto;
using System.Threading.Tasks;

namespace KTGKMD.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
