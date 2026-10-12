using Abp.Modules;
using Abp.Reflection.Extensions;
using KTGKMD.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace KTGKMD.Web.Startup;

[DependsOn(typeof(KTGKMDWebCoreModule))]
public class KTGKMDWebMvcModule : AbpModule
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfigurationRoot _appConfiguration;

    public KTGKMDWebMvcModule(IWebHostEnvironment env)
    {
        _env = env;
        _appConfiguration = env.GetAppConfiguration();
    }

    public override void PreInitialize()
    {
        Configuration.Navigation.Providers.Add<KTGKMDNavigationProvider>();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(KTGKMDWebMvcModule).GetAssembly());
    }
}
