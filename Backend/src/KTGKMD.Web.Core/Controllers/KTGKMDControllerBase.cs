using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace KTGKMD.Controllers
{
    public abstract class KTGKMDControllerBase : AbpController
    {
        protected KTGKMDControllerBase()
        {
            LocalizationSourceName = KTGKMDConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
