using System.Collections.Generic;
using System.Web.Mvc;

namespace DongTaoShop.Controllers
{
    public class RedirectController : Controller
    {
        // GET: Redirect
        public ActionResult Index(int? code)
        {
            if (code == 404)
            {
                Session["ThongBao"] = new List<string> { "404 Not Found" };
                return RedirectToAction("home", "index");
            }
            if (code == 3)
            {
                return RedirectToAction("index", "account");
            }
            if (code == 1)
            {
                return RedirectToAction("index", "home");
            }
            return RedirectToAction("index", "shop"); ;
        }
    }
}