using DongTaoShop.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DongTaoShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly DongTaoShopEntities db = new DongTaoShopEntities();
        // GET: Admin
        public ActionResult Index()
        {
            object token = Session["UserToken"];
            ViewBag.State = "Order";
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                TempData["DonHang"] = db.DonHangs.Where(x => x.TinhTrang == null).OrderBy(x => x.NgayDat).ToList();
                return View();
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }

        public ActionResult AcceptOrder(string token, int id)
        {
            if (CheckAdmin(token, db))
            {
                DonHang find = db.DonHangs.Where(x => x.Id == id).FirstOrDefault();
                if (find != null && find.TinhTrang == null)
                {
                    find.TinhTrang = "";
                    find.SanPham.LuotMua += 1;
                    find.SanPham.SoLuong -= 1;
                    db.SaveChanges();
                    Session["ThongBao"] = new List<string> { "Good Đơn hàng (DH-" + id + ") đã được duyệt" };
                    return RedirectToAction("index");

                }
                else
                {
                    Session["ThongBao"] = new List<string> { "Bad Đơn hàng (DH-" + id + ") không tồn tại hoặc đã được duyệt" };
                    return RedirectToAction("index");
                }
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }
        public ActionResult RefuseOrder(string token, int id)
        {
            if (CheckAdmin(token, db))
            {
                DonHang find = db.DonHangs.Where(x => x.Id == id).FirstOrDefault();
                if (find != null && find.TinhTrang == null)
                {
                    db.DonHangs.Remove(find);
                    db.SaveChanges();
                    Session["ThongBao"] = new List<string> { "Good Đơn hàng (DH-" + id + ") đã được hủy" };
                    return RedirectToAction("index");

                }
                else
                {
                    Session["ThongBao"] = new List<string> { "Bad Đơn hàng (DH-" + id + ") không tồn tại" };
                    return RedirectToAction("index");
                }
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }
        public static bool CheckAdmin(string token, DongTaoShopEntities db)
        {
            if (AccountController.OnLogin(token, db))
            {
                TaiKhoan find = db.TaiKhoans.Where(x => x.Token.Equals(token) && x.Admin == true).FirstOrDefault();
                if (find != null)
                {
                    return true;
                }
            }
            return false;
        }
        public ActionResult Product(int? page)
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                List<SanPham> list = new List<SanPham>();
                if (TempData["ListSanPham"] == null)
                {
                    list = db.SanPhams.OrderByDescending(x => x.Id).ToList();
                }
                else
                {
                    list = (List<SanPham>)TempData["ListSanPham"];
                }
                return View(list.ToPagedList(page ?? 1, 18));
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");

        }
        [HttpPost]
        public ActionResult SortProduct(FormCollection collection)
        {

            if (collection.Count > 0)
            {
                string selectSort = collection[0];
                List<SanPham> sort = new List<SanPham>();

                if (selectSort.Equals("Mặc định"))
                {
                    sort = db.SanPhams.ToList();
                }
                else if (selectSort.Equals("Vụ mùa"))
                {
                    DateTime dateNow = DateTime.Now;
                    sort = db.SanPhams.Where(b => b.VaoVu < dateNow.Month && b.RaVu > dateNow.Month).ToList();
                }
                else if (selectSort.Equals("Đặc sản"))
                {
                    sort = db.SanPhams.Where(b => b.DacSan == true).Where(c => c.SoLuong > 0).ToList();
                }
                else
                {
                    sort = db.SanPhams.Where(b => b.Loai.Contains(selectSort)).Where(c => c.SoLuong > 0).ToList();
                }
                TempData["ListSanPham"] = sort;
                TempData["Sort"] = selectSort;
                return RedirectToAction("product");
            }
            return RedirectToAction("index");
        }


        public ActionResult RefreshProduct()
        {
            string selectSort = null;
            if (TempData["Sort"] != null)
            {
                selectSort = (string)TempData["Sort"]; ;
            }
            else
            {
                selectSort = "Mặc định";
            }
            if (selectSort != null)
            {
                List<SanPham> sort = new List<SanPham>();

                if (selectSort.Equals("Mặc định"))
                {
                    sort = db.SanPhams.ToList();
                }
                else if (selectSort.Equals("Vụ mùa"))
                {
                    DateTime dateNow = DateTime.Now;
                    sort = db.SanPhams.Where(b => b.VaoVu < dateNow.Month && b.RaVu > dateNow.Month).ToList();
                }
                else if (selectSort.Equals("Đặc sản"))
                {
                    sort = db.SanPhams.Where(b => b.DacSan == true).Where(c => c.SoLuong > 0).ToList();
                }
                else
                {
                    sort = db.SanPhams.Where(b => b.Loai.Contains(selectSort)).Where(c => c.SoLuong > 0).ToList();
                }
                TempData["ListSanPham"] = sort;
                TempData["Sort"] = selectSort;
                return RedirectToAction("product");
            }
            return RedirectToAction("index");
        }


        [HttpPost]
        public ActionResult SearchProduct(FormCollection collection)
        {
            if (collection.Count > 0)
            {

                TempData["SelectSort"] = "check-shop-cur";
                string[] timkiem = collection[0].Split(' ');

                List<SanPham> listOut = new List<SanPham>();

                for (int i = 0; i < timkiem.Length; i++)
                {
                    string currentSearch = ShopController.RemoveSign(timkiem[i]);
                    var list = db.SanPhams.Where(x => x.TimKiem.Contains(currentSearch)).ToList();
                    foreach (var item in list)
                    {
                        listOut.Add(item);
                    }

                }

                var outOK = listOut.GroupBy(g => g).OrderByDescending(x => x.Count()).SelectMany(x => x).Distinct().ToList();

                TempData["ListSanPham"] = outOK;
                return RedirectToAction("product");
            }
            return RedirectToAction("index");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GoAddProduct(FormCollection collection, HttpPostedFileBase image)
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                if (collection.Count > 0)
                {
                    SanPham sanPham = new SanPham();
                    List<string> noti = new List<string>();
                    TaiKhoan user = db.TaiKhoans.Where(x => x.Token.Equals(token.ToString())).FirstOrDefault();

                    string ten = collection["ten"];
                    int? soluong = DataController.ToNullableInt(collection["soluong"]);
                    int? vaovu = DataController.ToNullableInt(collection["vaovu"]);
                    int? ravu = DataController.ToNullableInt(collection["ravu"]);
                    int? giamoi = DataController.ToNullableInt(collection["giamoi"]);
                    int? giacu = DataController.ToNullableInt(collection["giacu"]);
                    string loai = collection["loai"];
                    string dacsan = collection["dacsan"];
                    string xa = collection["xa"];
                    string huyen = collection["huyen"];
                    string tinh = collection["tinh"];
                    string goiy = collection["goiy"];
                    int donvitinh = int.Parse(collection["donvitinh"]);
                    string ghichu = collection["ghichu"];
                    int? landau = DataController.ToNullableInt(collection["landau"]);
                    int? kqg = DataController.ToNullableInt(collection["kqg"]);
                    int? kqm = DataController.ToNullableInt(collection["kqm"]);
                    int? kqmm = DataController.ToNullableInt(collection["kqmm"]);
                    int? mng = DataController.ToNullableInt(collection["mng"]);
                    int? mnl = DataController.ToNullableInt(collection["mnl"]);
                    string mota = collection["mota"];

                    int error = 0;
                    //Ten
                    if (ten.Length > 1)
                    {
                        sanPham.Ten = ten;
                    }
                    else
                    {
                        noti.Add("Bad Tên không được để trống và phải lớn hơn 1 ký tự");
                        error += 1;
                    }
                    // So luong
                    if (soluong == null)
                    {
                        sanPham.SoLuong = 10;
                    }
                    else
                    {
                        sanPham.SoLuong = soluong ?? default(int);
                    }
                    // Vu mua
                    if (vaovu != null && ravu != null)
                    {
                        if (vaovu > 0 && vaovu <= 12)
                        {
                            sanPham.VaoVu = vaovu ?? default(int);
                            sanPham.RaVu = ravu ?? default(int);
                        }
                        else
                        {
                            if (ravu != null && vaovu != null)
                            {
                                if (vaovu.Equals(""))
                                {
                                    noti.Add("Bad Nếu bạn để trống thì sản phẩm sẽ bày bán cả năm");

                                }
                                else if (ravu - vaovu < 0)
                                {
                                    noti.Add("Bad Tháng ra vụ phải lớn hơn hoặc bằng tháng vào vụ");

                                }
                                else
                                {
                                    noti.Add("Bad Tháng phải nằm trong khoảng 1-12");
                                }
                            }
                        }
                    }
                    //Gia ban
                    if (giacu != null && giamoi != null)
                    {
                        if (giamoi > 1000 && giacu > giamoi)
                        {
                            sanPham.GiaMoi = giamoi ?? default(int);
                            sanPham.GiaCu = giacu ?? default(int);
                        }
                        else
                        {
                            noti.Add("Bad Giá bán và giá quảng bá không được để trống, giá quảng bá phải lớn hơn giá mới");
                            error += 1;
                        }
                    }
                    //Loai mat hang
                    if (loai.Length > 0)
                    {
                        if (DataController.LoaiMatHang(DataController.ToNullableInt(loai)) != null)
                        {
                            sanPham.Loai = DataController.LoaiMatHang(DataController.ToNullableInt(loai));
                        }
                        else
                        {
                            noti.Add("Loại mặt hàng không đúng");
                            error += 1;
                        }
                    }
                    else
                    {
                        noti.Add("Loại mặt hàng không được bỏ trống");
                        error += 1;
                    }
                    //Dac san
                    if (dacsan.Equals("True") || dacsan.Equals("False"))
                    {
                        sanPham.DacSan = bool.Parse(dacsan);
                        if (bool.Parse(dacsan))
                        {
                            sanPham.DacSanD1 = xa.Length > 0 ? xa : null;
                            sanPham.DacSanD2 = huyen.Length > 0 ? huyen : null;
                            sanPham.DacSanD3 = tinh.Length > 0 ? tinh : null;
                            sanPham.GoiY = goiy.Length > 0 ? goiy : null;
                        }
                    }
                    //Don vi tinh
                    if (donvitinh >= 0)
                    {
                        sanPham.DonViTinh = DataController.DonViTinh(donvitinh);
                    }
                    //Ghi chu
                    sanPham.GhiChu = ghichu;
                    //Giam lan dau
                    if (landau != null)
                    {
                        if (landau >= 1 && landau <= 10)
                        {
                            sanPham.GLD = landau ?? default(int);
                        }
                        else
                        {
                            noti.Add("Bad Lưu ý % giảm từ 1 - 10");
                            error += 1;
                        }
                    }
                    //Giam khach quen
                    if (kqg != null && kqm != null && kqmm != null)
                    {
                        if (kqg >= 1 && kqg <= 10 && kqm >= 5 && kqm <= 50 && kqmm >= 5 && kqmm <= 50)
                        {
                            sanPham.TVG = kqg ?? default(int);
                            sanPham.TVM = kqm ?? default(int);
                            sanPham.TVSPLM = kqmm ?? default(int);
                        }
                        else
                        {
                            noti.Add("Bad Lưu ý % giảm từ 1 - 10, số lượng từ 5 - 50");
                            error += 1;
                        }
                    }
                    //Giam mua nhieu
                    if (mng != null && mnl != null)
                    {
                        if (mng >= 1 && mng <= 10 && mnl >= 5 && mnl <= 50)
                        {
                            sanPham.MNG = mng ?? default(int);
                            sanPham.MNSL = mnl ?? default(int);
                        }
                        else
                        {
                            error += 1;
                        }
                    }




                    //Mo ta
                    sanPham.MoTa = mota;
                    //ID
                    sanPham.IdNguoiBan = user.Id;
                    //Anh
                    if (image != null && image.ContentLength <= 1 * 1024 * 1024)
                    {
                        string imgName = ShopController.RemoveSign(sanPham.Ten).Replace(" ", "_") + ".png";

                        string path = Server.MapPath("~/Image/" + ShopController.RemoveSign(sanPham.Loai).Replace(" ", ""));

                        string fulPath = Path.Combine(path, imgName);

                        sanPham.Anh = "Image/" + ShopController.RemoveSign(sanPham.Loai).Replace(" ", "") + "/" + imgName;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        image.SaveAs(fulPath);
                    }
                    else
                    {
                        error += 1;
                        noti.Add("Bad Ảnh đại diện không được bỏ trống và phải nhỏ hơn 1MB");
                    }

                    if (error == 0)
                    {
                        noti.Add("Đã thêm sản phẩm mới");

                        //Timkiem

                        List<string> search = new List<string>();

                        if (dacsan.Equals("True"))
                        {
                            search.Add("dac san");
                        }
                        search.Add(ShopController.RemoveSign(sanPham.Ten.ToLower()));
                        search.Add(ShopController.RemoveSign(sanPham.Loai.ToLower()));

                        string contentSearch = null;
                        foreach (var item in search)
                        {
                            contentSearch += contentSearch + " " + item;
                        }

                        sanPham.TimKiem = contentSearch;


                        db.SanPhams.Add(sanPham);
                        db.SaveChanges();
                        Session["ThongBao"] = noti;
                        return RefreshProduct();
                    }
                    else
                    {
                        noti.Add("Bad Số vấn đề sảy ra " + error);
                        Session["ThongBao"] = noti;
                        return RedirectToAction("addproduct");
                    }

                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }
        public ActionResult AddProduct()
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                return View();
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }

        public ActionResult DetailProduct(int? id)
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                SanPham sp = db.SanPhams.Where(x => x.Id == id).FirstOrDefault();
                if (sp != null)
                {
                    db.SaveChanges();
                    TempData["SanPham"] = sp;
                    TempData["Hint"] = db.SanPhams.Where(x => x.Loai.Equals(sp.Loai) && x.Id != id).OrderByDescending(x => x.LuotMua).Take(12).ToList();
                    TempData["Dexuat"] = db.SanPhams.OrderByDescending(x => x.LuotMua).Take(12).ToList();
                    TempData["Best"] = db.SanPhams.OrderByDescending(x => x.LuotMua).Take(10).ToList();
                    return View();
                }
                else
                {
                    return RedirectToAction("product");
                }

            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }

        public ActionResult DeleteProduct(int? id)
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                SanPham sp = db.SanPhams.Where(x => x.Id == id).FirstOrDefault();
                if (sp != null)
                {
                    db.SanPhams.Remove(sp);
                    db.SaveChanges();
                    Session["ThongBao"] = new List<string> { "Good Sản phẩm đã được xóa" };
                    return RefreshProduct();
                }
                else
                {
                    Session["ThongBao"] = new List<string> { "Bad Sản phẩm không tồn tại" };
                    return RedirectToAction("product");
                }
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }

        public ActionResult EditProduct(int? id)
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                if (id != null)
                {
                    ViewBag.SanPham = db.SanPhams.Where(x => x.Id == id).FirstOrDefault();
                    return View();
                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GoEditProduct(FormCollection collection, HttpPostedFileBase image)
        {
            object token = Session["UserToken"];
            if (token != null && CheckAdmin(token.ToString(), db))
            {
                if (collection.Count > 0)
                {
                    List<string> noti = new List<string>();
                    TaiKhoan user = db.TaiKhoans.Where(x => x.Token.Equals(token.ToString())).FirstOrDefault();

                    string ten = collection["ten"];
                    int? soluong = DataController.ToNullableInt(collection["soluong"]);
                    int? vaovu = DataController.ToNullableInt(collection["vaovu"]);
                    int? ravu = DataController.ToNullableInt(collection["ravu"]);
                    int? giamoi = DataController.ToNullableInt(collection["giamoi"]);
                    int? giacu = DataController.ToNullableInt(collection["giacu"]);
                    string loai = collection["loai"];
                    string dacsan = collection["dacsan"];
                    string xa = collection["xa"];
                    string huyen = collection["huyen"];
                    string tinh = collection["tinh"];
                    string goiy = collection["goiy"];
                    int donvitinh = int.Parse(collection["donvitinh"]);
                    string ghichu = collection["ghichu"];
                    int? landau = DataController.ToNullableInt(collection["landau"]);
                    int? kqg = DataController.ToNullableInt(collection["kqg"]);
                    int? kqm = DataController.ToNullableInt(collection["kqm"]);
                    int? kqmm = DataController.ToNullableInt(collection["kqmm"]);
                    int? mng = DataController.ToNullableInt(collection["mng"]);
                    int? mnl = DataController.ToNullableInt(collection["mnl"]);
                    string mota = collection["mota"];
                    int? id = DataController.ToNullableInt(collection["id"]);


                    var sanpham = db.SanPhams.Where(x => x.Id == id).FirstOrDefault();


                    int error = 0;
                    //Ten
                    if (ten.Length > 1)
                    {
                        sanpham.Ten = ten;
                    }
                    else
                    {
                        noti.Add("Bad Tên không được để trống và phải lớn hơn 1 ký tự");
                        error += 1;
                    }
                    // So luong
                    if (soluong == null)
                    {
                        sanpham.SoLuong = 10;
                    }
                    else
                    {
                        sanpham.SoLuong = soluong ?? default(int);
                    }
                    // Vu mua
                    if (vaovu != null && ravu != null)
                    {
                        if (vaovu > 0 && vaovu <= 12)
                        {
                            sanpham.VaoVu = vaovu ?? default(int);
                            sanpham.RaVu = ravu ?? default(int);
                        }
                        else
                        {
                            if (ravu != null && vaovu != null)
                            {
                                if (vaovu.Equals(""))
                                {
                                    noti.Add("Bad Nếu bạn để trống thì sản phẩm sẽ bày bán cả năm");

                                }
                                else if (ravu - vaovu < 0)
                                {
                                    noti.Add("Bad Tháng ra vụ phải lớn hơn hoặc bằng tháng vào vụ");

                                }
                                else
                                {
                                    noti.Add("Bad Tháng phải nằm trong khoảng 1-12");
                                }
                            }
                        }
                    }
                    //Gia ban
                    if (giacu != null && giamoi != null)
                    {
                        if (giamoi > 1000 && giacu > giamoi)
                        {
                            sanpham.GiaMoi = giamoi ?? default(int);
                            sanpham.GiaCu = giacu ?? default(int);
                        }
                        else
                        {
                            noti.Add("Bad Giá bán và giá quảng bá không được để trống, giá quảng bá phải lớn hơn giá mới");
                            error += 1;
                        }
                    }
                    //Loai mat hang
                    if (loai.Length > 0)
                    {
                        if (DataController.LoaiMatHang(DataController.ToNullableInt(loai)) != null)
                        {
                            sanpham.Loai = DataController.LoaiMatHang(DataController.ToNullableInt(loai));
                        }
                        else
                        {
                            noti.Add("Loại mặt hàng không đúng");
                            error += 1;
                        }
                    }
                    else
                    {
                        noti.Add("Loại mặt hàng không được bỏ trống");
                        error += 1;
                    }
                    //Dac san
                    if (dacsan.Equals("True") || dacsan.Equals("False"))
                    {
                        sanpham.DacSan = bool.Parse(dacsan);
                        if (bool.Parse(dacsan))
                        {
                            sanpham.DacSanD1 = xa.Length > 0 ? xa : null;
                            sanpham.DacSanD2 = huyen.Length > 0 ? huyen : null;
                            sanpham.DacSanD3 = tinh.Length > 0 ? tinh : null;
                            sanpham.GoiY = goiy.Length > 0 ? goiy : null;
                        }
                    }
                    //Don vi tinh
                    if (donvitinh >= 0)
                    {
                        sanpham.DonViTinh = DataController.DonViTinh(donvitinh);
                    }
                    //Ghi chu
                    sanpham.GhiChu = ghichu;
                    //Giam lan dau
                    if (landau != null)
                    {
                        if (landau >= 1 && landau <= 10)
                        {
                            sanpham.GLD = landau ?? default(int);
                        }
                        else
                        {
                            noti.Add("Bad Lưu ý % giảm từ 1 - 10");
                            error += 1;
                        }
                    }
                    //Giam khach quen
                    if (kqg != null && kqm != null && kqmm != null)
                    {
                        if (kqg >= 1 && kqg <= 10 && kqm >= 5 && kqm <= 50 && kqmm >= 5 && kqmm <= 50)
                        {
                            sanpham.TVG = kqg ?? default(int);
                            sanpham.TVM = kqm ?? default(int);
                            sanpham.TVSPLM = kqmm ?? default(int);
                        }
                        else
                        {
                            noti.Add("Bad Lưu ý % giảm từ 1 - 10, số lượng từ 5 - 50");
                            error += 1;
                        }
                    }
                    //Giam mua nhieu
                    if (mng != null && mnl != null)
                    {
                        if (mng >= 1 && mng <= 10 && mnl >= 5 && mnl <= 50)
                        {
                            sanpham.MNG = mng ?? default(int);
                            sanpham.MNSL = mnl ?? default(int);
                        }
                        else
                        {
                            error += 1;
                        }
                    }
                    //Mo ta
                    sanpham.MoTa = mota;
                    //ID
                    sanpham.IdNguoiBan = user.Id;
                    //Anh
                    if (image != null && image.ContentLength <= 1 * 1024 * 1024)
                    {
                        string imgName = ShopController.RemoveSign(sanpham.Ten).Replace(" ", "_") + ".png";

                        string path = Server.MapPath("~/Image/" + ShopController.RemoveSign(sanpham.Loai).Replace(" ", ""));

                        string fulPath = Path.Combine(path, imgName);

                        sanpham.Anh = "Image/" + ShopController.RemoveSign(sanpham.Loai).Replace(" ", "") + "/" + imgName;

                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        image.SaveAs(fulPath);
                    }
                    else if (image != null && image.ContentLength > 1 * 1024 * 1024)
                    {
                        error += 1;
                        noti.Add("Bad Ảnh đại diện phải nhỏ hơn 1MB");
                    }

                    if (error == 0)
                    {
                        noti.Add("Good Cập nhật đã được lưu lại");


                        List<string> search = new List<string>();

                        if (dacsan.Equals("True"))
                        {
                            search.Add("dac san");
                        }
                        search.Add(ShopController.RemoveSign(sanpham.Ten.ToLower()));
                        search.Add(ShopController.RemoveSign(sanpham.Loai.ToLower()));

                        string contentSearch = null;
                        foreach (var item in search)
                        {
                            contentSearch += contentSearch + " " + item;
                        }

                        sanpham.TimKiem = contentSearch;


                        db.SaveChanges();
                        Session["ThongBao"] = noti;
                        return RefreshProduct();
                    }
                    else
                    {
                        noti.Add("Bad Số vấn đề sảy ra " + error);
                        Session["ThongBao"] = noti;
                        return RedirectToAction("editproduct", new { id = id });
                    }

                }
                else
                {
                    return RedirectToAction("index");
                }
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }

        public ActionResult Chart()
        {
            return View();
        }

        public ActionResult Account(int? page)
        {
            var token = Session["UserToken"];
            if (token != null && AccountController.OnLogin(token.ToString(), db))
            {
                var list = db.TaiKhoans.ToList();

                return View(list.ToPagedList(page ?? 1, 10));
            }


            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }
        [HttpPost]
        public async Task<ActionResult>  UpdateAccount(FormCollection collection)
        {
            var token = Session["UserToken"];
            if (token != null && AccountController.OnLogin(token.ToString(),db))
            {
                if (collection.Count == 3)
                {
                    var id = collection["id"].Split(',').ToArray();
                    var admin = collection["admin"].Split(',').ToArray();
                    var nhanvien = collection["staff"].Split(',').ToArray();

                    for (int i = 0; i < id.Length; i++)
                    {
                        var currentId = Int16.Parse(id[i]);
                        var account = db.TaiKhoans.Where(x => x.Id == currentId).FirstOrDefault();
                        account.Admin = Boolean.Parse(admin[i]);
                        account.NhanVien = Boolean.Parse(nhanvien[i]);
                        db.SaveChanges();
                    }
                   
                }
                Session["ThongBao"] = new List<string> { "Good Cập nhật thành công" };
                return RedirectToAction("account");
            }

            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");
        }

        public async Task<ActionResult> DeleteAccount(int? id)
        {
            var token = Session["UserToken"];
            if (id != null && token != null && AccountController.OnLogin(token.ToString(), db))
            {
                var user = db.TaiKhoans.Where(x => x.Id == id).FirstOrDefault();
                db.TaiKhoans.Remove(user);
                db.SaveChanges();
                Session["ThongBao"] = new List<string> { "Good Tài khoản đã được xóa" };
                return RedirectToAction("account");
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập đã hết hạn hoặc tài khoản không phải Admin" };
            return RedirectToAction("index", "account");

        }

    }

}