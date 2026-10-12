using Abp.Zero.EntityFrameworkCore;
using KTGKMD.Authorization.Roles;
using KTGKMD.Authorization.Users;
using KTGKMD.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace KTGKMD.EntityFrameworkCore;

public class KTGKMDDbContext : AbpZeroDbContext<Tenant, Role, User, KTGKMDDbContext>
{
    /* Define a DbSet for each entity of the application */

    public KTGKMDDbContext(DbContextOptions<KTGKMDDbContext> options)
        : base(options)
    {
    }
}
