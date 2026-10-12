using Abp.MultiTenancy;
using KTGKMD.Authorization.Users;

namespace KTGKMD.MultiTenancy;

public class Tenant : AbpTenant<User>
{
    public Tenant()
    {
    }

    public Tenant(string tenancyName, string name)
        : base(tenancyName, name)
    {
    }
}
