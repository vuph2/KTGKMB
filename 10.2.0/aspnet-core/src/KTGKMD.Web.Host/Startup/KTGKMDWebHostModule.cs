using Abp.Modules;
using Abp.Reflection.Extensions;
using KTGKMD.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace KTGKMD.Web.Host.Startup
{
    [DependsOn(
       typeof(KTGKMDWebCoreModule))]
    public class KTGKMDWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public KTGKMDWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(KTGKMDWebHostModule).GetAssembly());
        }
    }
}
