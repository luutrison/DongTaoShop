using DongTaoShop.Models;
using System.Linq;
using System.Web.Mvc;

namespace DongTaoShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DongTaoShopEntities db = new DongTaoShopEntities();
        public ActionResult Index()
        {
            TempData["Page"] = "Home";
            CreateIntroduceHome();
            return View();
        }

        private void CreateIntroduceHome()
        {
            IQueryable<SanPham> a = db.SanPhams.OrderByDescending(u => u.LuotMua.Value).Take(4);
            TempData["NoiBat"] = "Show";
            ViewBag.SanPhamIntro = a.ToList();
        }
    }
}