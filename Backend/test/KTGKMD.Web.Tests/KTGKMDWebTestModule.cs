using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using KTGKMD.EntityFrameworkCore;
using KTGKMD.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace KTGKMD.Web.Tests;

[DependsOn(
    typeof(KTGKMDWebMvcModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class KTGKMDWebTestModule : AbpModule
{
    public KTGKMDWebTestModule(KTGKMDEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
    }

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(KTGKMDWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(KTGKMDWebMvcModule).Assembly);
    }
}