using DongTaoShop.Controllers;
using DongTaoShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DongTaoShop.Areas.AccountAPI.Controllers
{
    public class AccountDataController : ApiController
    {
        private DongTaoShopEntities db = new DongTaoShopEntities();

        public IHttpActionResult Get(string token)
        {
            AccountAPI accountAPI = new AccountAPI();

            if (token != null && AccountController.OnLogin(token, db))
            {
                var find = db.TaiKhoans.Where(x => x.Token.Equals(token)).FirstOrDefault();
                if (find != null)
                {

                    accountAPI.Id = find.Id;
                    accountAPI.TenTK = find.TenTK;
                    accountAPI.MatKhau = find.MatKhau;
                    accountAPI.Admin = find.Admin;
                    accountAPI.Anh = find.Anh;
                    accountAPI.TenChuTK = find.TenChuTK;
                    accountAPI.NgaySinh = find.NgaySinh;
                    accountAPI.GioiTinh = find.GioiTinh;
                    accountAPI.CCCD = find.CCCD;
                    accountAPI.SDT = find.SDT;
                    accountAPI.SoNha = find.SoNha;
                    accountAPI.Phuong = find.Phuong;
                    accountAPI.Quan = find.Quan;
                    accountAPI.ThanhPho = find.ThanhPho;
                    accountAPI.GioiThieu = find.GioiThieu;
                    accountAPI.Token = find.Token;
                    accountAPI.Email = find.Email;
                }
                return Ok(accountAPI);
            }
            return BadRequest("null");
        }
    }
    public class AccountAPI
    {
        public int Id { get; set; }
        public string TenTK { get; set; }
        public string MatKhau { get; set; }
        public Nullable<bool> Admin { get; set; }
        public string Anh { get; set; }
        public string TenChuTK { get; set; }
        public Nullable<System.DateTime> NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string CCCD { get; set; }
        public string SDT { get; set; }
        public string SoNha { get; set; }
        public string Phuong { get; set; }
        public string Quan { get; set; }
        public string ThanhPho { get; set; }
        public string GioiThieu { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
