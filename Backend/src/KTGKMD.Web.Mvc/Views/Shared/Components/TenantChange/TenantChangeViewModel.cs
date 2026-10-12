using Abp.AutoMapper;
using KTGKMD.Sessions.Dto;

namespace KTGKMD.Web.Views.Shared.Components.TenantChange;

[AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
public class TenantChangeViewModel
{
    public TenantLoginInfoDto Tenant { get; set; }
}
