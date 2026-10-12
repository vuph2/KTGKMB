using Abp.Application.Services;
using KTGKMD.MultiTenancy.Dto;

namespace KTGKMD.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

