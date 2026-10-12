using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using KTGKMD.Authorization;

namespace KTGKMD;

[DependsOn(
    typeof(KTGKMDCoreModule),
    typeof(AbpAutoMapperModule))]
public class KTGKMDApplicationModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Authorization.Providers.Add<KTGKMDAuthorizationProvider>();
    }

    public override void Initialize()
    {
        var thisAssembly = typeof(KTGKMDApplicationModule).GetAssembly();

        IocManager.RegisterAssemblyByConvention(thisAssembly);

        Configuration.Modules.AbpAutoMapper().Configurators.Add(
            // Scan the assembly for classes which inherit from AutoMapper.Profile
            cfg => cfg.AddMaps(thisAssembly)
        );
    }
}
