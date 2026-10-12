using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using KTGKMD.Configuration;
using KTGKMD.EntityFrameworkCore;
using KTGKMD.Migrator.DependencyInjection;
using Castle.MicroKernel.Registration;
using Microsoft.Extensions.Configuration;

namespace KTGKMD.Migrator;

[DependsOn(typeof(KTGKMDEntityFrameworkModule))]
public class KTGKMDMigratorModule : AbpModule
{
    private readonly IConfigurationRoot _appConfiguration;

    public KTGKMDMigratorModule(KTGKMDEntityFrameworkModule abpProjectNameEntityFrameworkModule)
    {
        abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

        _appConfiguration = AppConfigurations.Get(
            typeof(KTGKMDMigratorModule).GetAssembly().GetDirectoryPathOrNull()
        );
    }

    public override void PreInitialize()
    {
        Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
            KTGKMDConsts.ConnectionStringName
        );

        Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
        Configuration.ReplaceService(
            typeof(IEventBus),
            () => IocManager.IocContainer.Register(
                Component.For<IEventBus>().Instance(NullEventBus.Instance)
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(KTGKMDMigratorModule).GetAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
}
