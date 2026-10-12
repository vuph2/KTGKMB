using Abp.AspNetCore.Mvc.Authorization;
using KTGKMD.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KTGKMD.Web.Controllers;

[AbpMvcAuthorize]
public class AboutController : KTGKMDControllerBase
{
    public ActionResult Index()
    {
        return View();
    }
}
