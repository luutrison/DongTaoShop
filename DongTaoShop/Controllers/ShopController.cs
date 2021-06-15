using DongTaoShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DongTaoShop.Controllers
{
    public class ShopController : Controller
    {
        private readonly DongTaoShopEntities db = new DongTaoShopEntities();
        public ActionResult Index(int? page, int? redirect)
        {
            TempData["Page"] = "Shop";
            List<SanPham> a = new List<SanPham>();
            if (TempData["ListShop"] == null)
            {
                a = DefaulseListShop();
                TempData["ListShop"] = a;
            }
            else
            {
                a = (List<SanPham>)TempData["ListShop"];
            }

            if (redirect == 1)
            {
                Notification("Good Bạn đã đặt hàng thành công, chúng tôi sẽ sớm liên lạc lại với bạn");
            }
            return View(a.ToPagedList(page ?? 1, 18));
        }

        [HttpPost]
        public ActionResult SortData(FormCollection collection)
        {
            if (collection.Count > 1)
            {
                List<SanPham> sort = new List<SanPham>();

                string selectSort = collection[0];

                int typeSort = short.Parse(collection[1]);

                if (selectSort.Equals("Current"))
                {
                    sort = (List<SanPham>)TempData["ListShop"];
                    TempData["NoiBat"] = null;
                }
                else if (selectSort.Equals("Nổi bật"))
                {
                    sort = db.SanPhams.Where(b => b.LuotMua != 0).Where(c => c.SoLuong > 0).Take(36).ToList();
                    TempData["NoiBat"] = "Show";
                }
                else if (selectSort.Equals("Vụ mùa"))
                {
                    DateTime dateNow = DateTime.Now;
                    sort = db.SanPhams.Where(b => b.VaoVu < dateNow.Month && b.RaVu > dateNow.Month).ToList();
                    TempData["NoiBat"] = null;
                }
                else if (selectSort.Equals("Đặc sản"))
                {
                    sort = db.SanPhams.Where(b => b.DacSan == true).Where(c => c.SoLuong > 0).ToList();
                    TempData["NoiBat"] = null;
                }
                else
                {
                    sort = db.SanPhams.Where(b => b.Loai.Contains(selectSort)).Where(c => c.SoLuong > 0).ToList();
                    TempData["NoiBat"] = null;
                }
                TempData["SelectSort"] = selectSort;
                TempData["TypeSort"] = typeSort;
                TempData["ListShop"] = SortSanPhamBy(sort, typeSort);
            }
            else
            {
                TempData["ListShop"] = SortSanPhamBy((List<SanPham>)TempData["ListShop"], Int16.Parse(collection[0]));
                TempData["TypeSort"] = Int16.Parse(collection[0]);
            }

            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult SearchShop(FormCollection collection)
        {
            if (collection.Count > 0)
            {
                List<SanPham> listSearch = new List<SanPham>();
                List<SanPham> allList = db.SanPhams.ToList();
                TempData["SelectSort"] = "check-shop-cur";
                string[] search = collection[0].Split(' ');

                foreach (SanPham item in allList)
                {

                    for (int i = 0; i < search.Length; i++)
                    {
                        if (RemoveSign(item.Ten).ToLower().Contains(RemoveSign(search[i]).ToLower()))
                        {
                            listSearch.Add(item);
                        }
                    }
                }
                List<SanPham> list = listSearch.GroupBy(g => g).OrderByDescending(x => x.Count()).SelectMany(x => x).Distinct().ToList();
                TempData["ListShop"] = list;
            }
            return RedirectToAction("index");
        }
        [HttpPost]
        public ActionResult GoCart(FormCollection collection)
        {
            if (collection.Count > 0)
            {
                List<string> name = collection[0].Split(',').ToList();
                List<string> number = collection[1].Split(',').ToList();
                List<GioHangShop> listRaw = new List<GioHangShop>();
                List<GioHangShop> listSort = new List<GioHangShop>();
                List<SanPham> listFinal = new List<SanPham>();
                List<int> numICart = new List<int>();


                for (int i = 0; i < name.Count; i++)
                {
                    GioHangShop item = new GioHangShop
                    {
                        name = name[i].ToString(),
                        number = short.Parse(number[i])
                    };
                    listRaw.Add(item);
                }

                for (int i = 0; i < listRaw.Count; i++)
                {
                    List<GioHangShop> listTemp = listRaw;
                    for (int j = i + 1; j < listRaw.Count; j++)
                    {
                        if (listRaw[i].name.Equals(listRaw[j].name))
                        {
                            listRaw[i].number = listRaw[i].number + listRaw[j].number;
                        }
                    }
                    List<GioHangShop> tempList = listRaw.Where(x => x.name.Equals(listRaw[i].name)).ToList();
                    tempList.RemoveAll(x => x.number < listRaw[i].number);
                    listSort.Add(tempList[0]);
                }
                List<GioHangShop> listOut = listSort.Distinct().ToList();

                foreach (GioHangShop item in listOut)
                {
                    if (!item.name.Equals("") || !item.name.Equals(" "))
                    {
                        List<SanPham> find = db.SanPhams.Where(x => x.Ten.Equals(item.name)).ToList();
                        if (find.Any())
                        {
                            listFinal.Add(find[0]);
                            numICart.Add(item.number);
                        }
                    }
                }
                TempData["ListCart"] = listFinal;
                TempData["ListCartNumber"] = numICart;
                object login = Session["UserToken"];
                if (login != null)
                {
                    return RedirectToAction("paycart");
                }
            }
            else
            {
                return RedirectToAction("index");
            }
            return RedirectToAction("paycartgo");
        }

        public ActionResult PayCart()
        {
            if (TempData["ListCart"] != null)
            {
                object token = Session["UserToken"];
                if (token != null && AccountController.OnLogin(token.ToString(), db))
                {
                    TempData["Page"] = "Shop";
                    TaiKhoan findUser = db.TaiKhoans.Where(x => x.Token.Equals(token.ToString())).FirstOrDefault();
                    List<SanPham> listSP = (List<SanPham>)TempData["ListCart"];
                    List<int> listSL = (List<int>)TempData["ListCartNumber"];
                    List<KhuyenMai> listKM = new List<KhuyenMai>();
                    for (int i = 0; i < listSP.Count; i++)
                    {
                        int idSanPham = listSP[i].Id;
                        int? tvLuotMua = listSP[i].TVSPLM != null ? listSP[i].TVSPLM : 0; //tam gan bang 0 de ko bi loi
                        KhuyenMai khuyenMai = new KhuyenMai();
                        DonHang findLanDau = db.DonHangs.Where(x => x.IdNguoiMua == findUser.Id && x.IdSanPham == idSanPham).FirstOrDefault();
                        List<DonHang> findThanhVien = db.DonHangs.Where(x => x.IdNguoiMua == findUser.Id && x.IdSanPham == idSanPham && x.SoLuong >= tvLuotMua).ToList();
                        if (findLanDau == null && listSP[i].GLD != null && listSP[i].GLD > 0)
                        {
                            khuyenMai.MuaLanDau = true;
                        }
                        if (listSP[i].MNSL != null && listSP[i].MNSL > 0 && listSL[i] >= listSP[i].MNSL)
                        {
                            khuyenMai.MuaNhieu = true;
                        }
                        if (listSP[i].TVG != null && listSP[i].TVG > 0 && findThanhVien.Count >= listSP[i].TVM)
                        {
                            khuyenMai.ThanhVien = true;
                        }
                        listKM.Add(khuyenMai);
                    }
                    TempData["ListKhuyenMai"] = listKM;
                }
                else
                {
                    Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn, hoặc ai đó đang đăng nhập tài khoản của bạn" };
                    Session["UserToken"] = null;
                    return RedirectToAction("index", "account");
                }
            }
            else
            {
                return RedirectToAction("index");
            }
            return View();
        }
        public ActionResult PayCartGo()
        {
            TempData["Page"] = "Shop";
            if (TempData["ListCart"] != null)
            {
                Session["ThongBao"] = new List<string> { "Bad Bạn sẽ không nhận được khuyến mãi khi không dùng tài khoản" };
                return View();
            }
            else
            {
                return RedirectToAction("index");
            }

        }
        [HttpPost]
        public ActionResult FinishPayCart(FormCollection collection)
        {
            List<string> thongBao = new List<string>();
            object token = Session["UserToken"];

            if (token != null && AccountController.OnLogin(token.ToString(), db))
            {
                if (collection.Count > 0)
                {
                    TaiKhoan findUser = db.TaiKhoans.Where(x => x.Token.Equals(token.ToString())).FirstOrDefault();
                    if (findUser == null)
                    {
                        Session["UserToken"] = null;
                        thongBao.Add("Bad Xác thực không trùng khớp, mời bạn đăng nhập lại");
                        Session["ThongBao"] = thongBao;
                        return RedirectToAction("index", "account");
                    }
                    else
                    {
                        if (findUser.SDT.Length == 0 || findUser.Phuong.Length == 0 || findUser.Quan.Length == 0 || findUser.ThanhPho.Length == 0 || findUser.SoNha.Length == 0)
                        {
                            thongBao.Add("Bad Bạn cần điền đủ thông tin cá nhân để chúng tôi xác định vị trí gửi hàng cho bạn");
                            Session["ThongBao"] = thongBao;
                            return RedirectToAction("index", "account");
                        }
                        else
                        {
                            string[] idSanPham = collection["idSanPham"].Split(',');
                            string[] soluong = collection["soluong"].Split(',');
                            int idNguoiMua = findUser.Id;
                            //add to order
                            for (int i = 0; i < idSanPham.Length; i++)
                            {
                                int km = 0;
                                int idSP = int.Parse(idSanPham[i]);
                                int sl = int.Parse(soluong[i]);
                                KhuyenMai khuyenMai = new KhuyenMai()
                                {
                                    ThanhVien = false,
                                    MuaLanDau = false,
                                    MuaNhieu = false,
                                };
                                DonHang checkMuaDau = db.DonHangs.Where(x => x.IdNguoiMua == idNguoiMua && x.IdSanPham == idSP).FirstOrDefault();
                                SanPham findSanPham = db.SanPhams.Where(x => x.Id == idSP).FirstOrDefault();
                                List<DonHang> checkThanhVien = db.DonHangs.Where(x => x.IdNguoiMua == idNguoiMua && x.IdSanPham == idSP && x.SoLuong >= x.SanPham.TVM).ToList();
                                if (checkMuaDau == null)
                                {
                                    khuyenMai.MuaLanDau = true;
                                    km += findSanPham.GLD ?? default(int);
                                }
                                if (checkThanhVien.Count >= findSanPham.TVM)
                                {
                                    khuyenMai.ThanhVien = true;
                                    km += findSanPham.TVG ?? default(int);
                                }
                                if (sl >= findSanPham.MNSL)
                                {
                                    khuyenMai.MuaNhieu = true;
                                    km += findSanPham.MNG ?? default(int);
                                }
                                int tongCong = findSanPham.GiaMoi * sl - (findSanPham.GiaMoi * sl * km / 100) ?? default(int);
                                DonHang donHang = new DonHang()
                                {
                                    IdNguoiMua = idNguoiMua,
                                    IdSanPham = idSP,
                                    MuaLanDau = khuyenMai.MuaLanDau,
                                    MuaNhieu = khuyenMai.MuaNhieu,
                                    ThanhVien = khuyenMai.ThanhVien,
                                    NgayDat = DateTime.Now,
                                    SoLuong = sl,
                                    TongCong = tongCong,
                                };
                                db.DonHangs.Add(donHang);
                                db.SaveChanges();
                            }

                            TempData["ClearCart"] = "OK";
                            thongBao.Add("Good Đặt hàng thành công, chúng tôi sẽ sớm kiểm tra và liên hệ lại với bạn");
                        }

                    }
                }

            }
            else
            {
                thongBao.Add("Bad Bạn cần đăng nhập lại để thực hiện thao tác này");
                Session["ThongBao"] = thongBao;
                Session["UserToken"] = null;
                return RedirectToAction("index", "account");
            }
            Session["ThongBao"] = thongBao;
            return RedirectToAction("index", "shop");
        }

        public ActionResult DetailProduct(int? id)
        {
            SanPham sp = db.SanPhams.Where(x => x.Id == id).FirstOrDefault();

            if (sp == null)
            {
                return RedirectToAction("index");
            }
            else
            {

                TempData["SanPham"] = sp;
                TempData["Hint"] = db.SanPhams.Where(x => x.Loai.Equals(sp.Loai) && x.Id != id).OrderByDescending(x => x.LuotMua).Take(12).ToList();
                TempData["Dexuat"] = db.SanPhams.OrderByDescending(x => x.LuotMua).Take(12).ToList();
                TempData["Best"] = db.SanPhams.OrderByDescending(x => x.LuotMua).Take(10).ToList();
                return View();
            }
        }

        public ActionResult ShowProductType(string loai)
        {
            List<SanPham> list = db.SanPhams.Where(x => x.Loai.Equals(loai)).ToList();

            if (list.Count > 0)
            {
                TempData["ListShop"] = list;
                TempData["SelectSort"] = loai;
                return RedirectToAction("index");
            }
            return RedirectToAction("index");

        }

        //Function other
        private List<SanPham> SortSanPhamBy(List<SanPham> list, int a)
        {
            List<SanPham> listNew = new List<SanPham>();
            if (a == 0)
            {
                listNew = list.OrderByDescending(b => b.LuotMua).ToList();
            }
            else if (a == 1)
            {
                listNew = list.OrderBy(b => b.GiaMoi).ToList();
            }
            else if (a == 2)
            {
                listNew = list.OrderByDescending(b => b.GiaMoi).ToList();
            }
            else if (a == 3)
            {
                listNew = list.OrderBy(b => b.Ten).ToList();
            }
            return listNew;
        }
        public static string RemoveSign(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                {
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
                }
            }
            return str;
        }
        private static readonly string[] VietnameseSigns = new string[]
      {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
      };


        private List<SanPham> DefaulseListShop()
        {
            List<SanPham> list = db.SanPhams.OrderByDescending(b => b.LuotMua).Where(c => c.SoLuong > 0).ToList();
            return list;

        }

        public void Notification(string notification)
        {
            List<string> noti = new List<string>
            {
                notification
            };
            Session["ThongBao"] = noti;
        }
    }

    public class GioHangShop
    {
        public string name { get; set; }
        public int number { get; set; }
    }
    public class KhuyenMai
    {
        public bool MuaLanDau { get; set; }
        public bool MuaNhieu { get; set; }
        public bool ThanhVien { get; set; }
    }
}