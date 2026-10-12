using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace KTGKMD.Web.Views;

public abstract class KTGKMDRazorPage<TModel> : AbpRazorPage<TModel>
{
    [RazorInject]
    public IAbpSession AbpSession { get; set; }

    protected KTGKMDRazorPage()
    {
        LocalizationSourceName = KTGKMDConsts.LocalizationSourceName;
    }
}
