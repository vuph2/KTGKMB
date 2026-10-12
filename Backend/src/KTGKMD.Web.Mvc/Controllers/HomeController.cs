using KTGKMD.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KTGKMD.Web.Controllers;

public class HomeController : KTGKMDControllerBase
{
    public ActionResult Index()
    {
        return RedirectToAction("Index", "Exams");
    }
}
