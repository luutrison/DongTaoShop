using DongTaoShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DongTaoShop.Controllers
{
    public class AccountController : Controller
    {
        private static readonly Random random = new Random();
        private readonly DongTaoShopEntities db = new DongTaoShopEntities();
        public ActionResult Index()
        {
            TempData["Page"] = "Account";
            object token = Session["UserToken"];
            if (token != null && OnLogin(token.ToString(), db))
            {
                return RedirectToAction("userprofile", new { user = token });
            }
            if (token != null && !OnLogin(token.ToString(), db))
            {
                Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập của bạn đã hết hạn hoặc có ai đó đã đăng nhập vào tài khoản của bạn" };
            }
            return View();
        }

        public ActionResult RegisterView()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            List<string> thongBao = new List<string>();
            if (collection.Count != 0)
            {
                string account = collection[0].Trim();
                string password = collection[1].Trim();
                string repass = collection[2].Trim();
                string email = collection[3].Trim();
                string reCaptcha = collection["recaptcha"];
                int validEror = 0;
                List<string> invalidChars = new List<string>() { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", " " };

                if (Recaptcha.ValidationRecaptcha(reCaptcha, Request.ServerVariables["REMOTE_ADDR"]))
                {
                    if (!account.Equals("") && !password.Equals("") && account.Length > 6 && password.Length > 6)
                    {
                        foreach (string item in invalidChars)
                        {
                            if (account.Contains(item))
                            {
                                thongBao.Add("Bad Tài khoản chứa ký tự không hợp lệ");
                                validEror += 1;
                            }
                        }
                        if (validEror == 0)
                        {
                            IQueryable<TaiKhoan> find = db.TaiKhoans.Where(x => x.TenTK.Equals(account));
                            if (!find.Any())
                            {
                                if (password.Equals(repass))
                                {
                                    TaiKhoan taiKhoan = new TaiKhoan()
                                    {
                                        TenTK = account,
                                        MatKhau = password,
                                        Email = email
                                    };
                                    thongBao.Add("Good Đăng ký thành công");
                                    db.TaiKhoans.Add(taiKhoan);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    thongBao.Add("Bad Mật khẩu không trùng khớp");
                                }
                            }
                            else
                            {
                                thongBao.Add("Bad Tài khoản đã tồn tại");
                            }
                        }
                    }
                    else
                    {
                        thongBao.Add("Bad Tài khoản, mật khẩu không được để trống");
                    }
                }
                else
                {
                    thongBao.Add("Bad Xác thực không đúng");
                }
            }
            Session["ThongBao"] = thongBao;
            return RedirectToAction("registerview");
        }
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            List<string> thongBao = new List<string>();
            if (collection.Count != 0)
            {
                string account = collection[0].Trim();
                string password = collection[1].Trim();
                string reCaptcha = collection["recaptcha"];
                string hostname = Request.ServerVariables["REMOTE_ADDR"];
                if (Recaptcha.ValidationRecaptcha(reCaptcha, hostname))
                {
                    List<TaiKhoan> find = db.TaiKhoans.Where(x => x.TenTK.Equals(account)).ToList();
                    if (find.Any() && find[0].MatKhau.Equals(password))
                    {
                        CreateToken(find[0]);

                        Session["UserToken"] = find[0].Token;
                        thongBao.Add("Good Đăng nhập thành công");
                        Session["ThongBao"] = thongBao;
                        Session.Timeout = 1440;
                        return RedirectToAction("index", "shop");
                    }
                    else
                    {
                        thongBao.Add("Bad Tài khoản hoặc mật khẩu không đúng");
                    }
                }
                else
                {
                    thongBao.Add("Bad Xác thực không đúng");
                }
            }
            Session["ThongBao"] = thongBao;
            Session.Timeout = 1440;
            return RedirectToAction("index");
        }
        public ActionResult UserProfile()
        {
            object token = Session["UserToken"];
            if (token != null && AccountController.OnLogin(token.ToString(), db))
            {
                TaiKhoan find = db.TaiKhoans.Where(x => x.Token.Equals(token.ToString())).FirstOrDefault();
                TempData["UserProfile"] = find;
                return View();
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập của bạn đã hết hạn hoặc có ai đó đã đăng nhập vào tài khoản của bạn" };
            Session["UserToken"] = null;
            return RedirectToAction("index");
        }
        [HttpPost]
        public ActionResult UpdateProfile(FormCollection collection, HttpPostedFileBase image)
        {
            object token = Session["UserToken"];
            if (collection.Count > 0 || image != null)
            {
                if (token != null && AccountController.OnLogin(token.ToString(), db))
                {
                    string name = collection["ten"];
                    string sdt = collection["sdt"];
                    DateTime date = DateTime.ParseExact(collection["date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    int sex = short.Parse(collection["sex"]);
                    string cmt = collection["cmt"];
                    string sonha = collection["sonha"];
                    string phuong = collection["phuong"];
                    string quan = collection["quan"];
                    string thanhpho = collection["thanhpho"];
                    int id = short.Parse(collection["id"]);
                    string oldpass = collection["oldpass"];
                    string newpass = collection["newpass"];
                    string repass = collection["repass"];
                    List<string> thongBao = new List<string>();
                    List<TaiKhoan> find = db.TaiKhoans.Where(x => x.Id == id).ToList();

                    if (image != null && image.ContentLength < 1 * 1024 * 1024)
                    {
                        string imgName = "avatar_" + find[0].TenTK + ".png";

                        string path = Server.MapPath("~/Image/Avatar");
                        string fulPath = Path.Combine(Server.MapPath("~/Image/Avatar"), imgName);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        image.SaveAs(fulPath);
                        find[0].Anh = "Image/Avatar/" + imgName;
                        db.SaveChanges();
                    }
                    else
                    {
                        thongBao.Add("Bad Ảnh đại diện phải nhỏ hơn 1MB");
                    }
                    if (!name.Equals(""))
                    {
                        find[0].TenChuTK = name;
                        db.SaveChanges();
                    }
                    if (!sdt.Equals(""))
                    {
                        find[0].SDT = sdt;
                        db.SaveChanges();
                    }
                    if (!date.Equals(""))
                    {
                        find[0].NgaySinh = date;
                        db.SaveChanges();
                    }
                    if (!sex.Equals(""))
                    {
                        find[0].GioiTinh = DataController.GioiTinh(sex);
                        db.SaveChanges();
                    }
                    if (!cmt.Equals(""))
                    {
                        find[0].CCCD = cmt;
                        db.SaveChanges();
                    }
                    if (!sonha.Equals(""))
                    {
                        find[0].SoNha = sonha;
                        db.SaveChanges();
                    }
                    if (!phuong.Equals(""))
                    {
                        find[0].Phuong = phuong;
                        db.SaveChanges();
                    }
                    if (!quan.Equals(""))
                    {
                        find[0].Quan = quan;
                        db.SaveChanges();
                    }
                    if (!thanhpho.Equals(""))
                    {
                        find[0].ThanhPho = thanhpho;
                        db.SaveChanges();
                    }
                    //Đổi mật khẩu
                    if (!oldpass.Equals("") && !newpass.Equals("") && !repass.Equals(""))
                    {
                        if (find[0].MatKhau.Equals(oldpass))
                        {
                            if (newpass.Equals(repass))
                            {
                                find[0].MatKhau = newpass;
                                db.SaveChanges();
                                Session["UserToken"] = null;
                                thongBao.Add("Good Thay đổi mật khẩu thành công");
                                Session["ThongBao"] = thongBao;
                                return RedirectToAction("index", "account");
                            }
                            else
                            {
                                thongBao.Add("Bad Mật khẩu không khớp");
                            }
                        }
                        else
                        {
                            thongBao.Add("Bad Không trùng mật khẩu cũ");
                        }
                    }
                    thongBao.Add("Good Mọi thay đổi đã được lưu lại");
                    Session["ThongBao"] = thongBao;
                    return RedirectToAction("UserProfile", new { user = Session["UserToken"].ToString() });
                }
            }
            return RedirectToAction("index", "shop");
        }
        public ActionResult LogOut()
        {
            Session["UserToken"] = null;
            Session["ThongBao"] = new List<string> { "Good Đăng xuất thành công" };
            return RedirectToAction("index");
        }
        public ActionResult OutTimeSkipLogin()
        {
            object token = Session["UserToken"];
            if (token != null && !OnLogin(token.ToString(), db))
            {
                Session["UserToken"] = null;
                Session["ThongBao"] = new List<string> { "Bad Bạn đã bỏ qua đăng nhập, bạn sẽ không được hưởng khuyến mãi khi mua bất kì sản phẩm nào" };
                return RedirectToAction("index", "shop");
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        public ActionResult Order()
        {
            object token = Session["UserToken"];
            if (token != null && OnLogin(token.ToString(), db))
            {
                TaiKhoan user = db.TaiKhoans.Where(x => x.Token.Equals(token.ToString())).FirstOrDefault();
                List<DonHang> list = db.DonHangs.Where(x => x.TaiKhoan.Id == user.Id).ToList();
                ViewBag.DonHang = list;
                ViewBag.User = user;
                return View();
            }
            Session["ThongBao"] = new List<string> { "Bad Phiên đăng nhập của bạn đã hết hạn hoặc có ai đó đã đăng nhập vào tài khoản của bạn" };
            Session["UserToken"] = null;
            return RedirectToAction("index");
        }


        public void CreateToken(TaiKhoan taiKhoan)
        {
            DateTime now = DateTime.Now;
            string token = Base64Encode(taiKhoan.TenTK) + "%" + RandomString(200) + "%" + Base64Encode(now.ToString());
            taiKhoan.Token = token;
            db.SaveChanges();
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789qwertyuiopasdfghjklzxcvbnm";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string Base64Encode(string plainText)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        private static bool OutDateToken(string token)
        {
            DateTime origin = new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            double nowSecond = Math.Floor((DateTime.Now.ToUniversalTime() - origin).TotalSeconds);
            string stringDate64 = token.ToString().Split('%').ToList()[2];
            DateTime dateToken = DateTime.ParseExact(AccountController.Base64Decode(stringDate64), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            double dateTokenSencond = Math.Floor((dateToken.ToUniversalTime() - origin).TotalSeconds);
            if (nowSecond - dateTokenSencond > 86400)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool OnLogin(string token, DongTaoShopEntities db)
        {
            TaiKhoan login = db.TaiKhoans.Where(x => x.Token.Equals(token)).FirstOrDefault();
            if (login != null && !OutDateToken(token))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class GoogleRecaptcha
    {
        public bool Success { get; set; }
        public DateTime Time { get; set; }
        public string Hostname { get; set; }
    }
    public class Recaptcha
    {
        public static bool ValidationRecaptcha(string tokenCaptcha, string hostname)
        {
            if (tokenCaptcha != null)
            {
                string secretKey = "6LdoHhYbAAAAAKWWBUOlRQa_RuBAMa23DL7YY8ZV";//Mã bí mật
                /*string hostname = Request.ServerVariables["REMOTE_ADDR"];*/
                string myParameters = string.Format("secret={0}&response={1}&remoteip={2}", secretKey, tokenCaptcha, hostname);
                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string json = wc.UploadString("https://www.google.com/recaptcha/api/siteverify", myParameters);
                    GoogleRecaptcha js = JsonConvert.DeserializeObject<GoogleRecaptcha>(json);
                    if (js.Success)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}