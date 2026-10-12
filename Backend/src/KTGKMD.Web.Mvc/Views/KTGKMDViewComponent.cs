using Abp.AspNetCore.Mvc.ViewComponents;

namespace KTGKMD.Web.Views;

public abstract class KTGKMDViewComponent : AbpViewComponent
{
    protected KTGKMDViewComponent()
    {
        LocalizationSourceName = KTGKMDConsts.LocalizationSourceName;
    }
}
