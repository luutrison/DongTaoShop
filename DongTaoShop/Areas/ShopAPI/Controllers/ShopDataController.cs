using DongTaoShop.Controllers;
using DongTaoShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DongTaoShop.Areas.ShopAPI.Controllers
{
    public class ShopDataController : ApiController
    {
        private DongTaoShopEntities db = new DongTaoShopEntities();
        public IHttpActionResult GetProductData(int id)
        {
            var sanpham = db.SanPhams.OrderBy(x => x.Id == id).FirstOrDefault();

            SanPhamAPI sp = new SanPhamAPI();
            sp.Id = sanpham.Id;
            sp.Ten = sanpham.Ten;
            sp.GiaMoi = sanpham.GiaMoi;
            sp.GiaCu = sanpham.GiaCu;
            sp.MoTa = sanpham.MoTa;
            sp.Loai = sanpham.Loai;
            sp.IdNguoiBan = sanpham.IdNguoiBan;
            sp.TenNguoiBan = sanpham.TaiKhoan.TenChuTK;
            sp.Anh = sanpham.Anh;
            sp.LuotMua = sanpham.LuotMua;
            sp.SoLuong = sanpham.SoLuong;
            sp.DacSan = sanpham.DacSan;
            sp.DacSanD1 = sanpham.DacSanD1;
            sp.DacSanD2 = sanpham.DacSanD2;
            sp.DacSanD3 = sanpham.DacSanD3;
            sp.VaoVu = sanpham.VaoVu;
            sp.RaVu = sanpham.RaVu;
            sp.GLD = sanpham.GLD;
            sp.MNSL = sanpham.MNSL;
            sp.MNG = sanpham.MNG;
            sp.TVG = sanpham.TVG;
            sp.TVM = sanpham.TVM;
            sp.TVSPLM = sanpham.TVSPLM;
            sp.DonViTinh = sanpham.DonViTinh;
            sp.GoiY = sanpham.GoiY;
            sp.GhiChu = sanpham.GhiChu;

            return Ok(sp);
        }

        [HttpGet]
        [Route("api/shopapi/shopdata/sanphamitem/{index}")]
        public IHttpActionResult SanPhamItem(int index)
        {
            var sanpham = db.SanPhams.OrderBy(x => x.Id).Skip(index).ToList();
            List<SanPhamShop> list = new List<SanPhamShop>();
            foreach (var item in sanpham)
            {
                SanPhamShop sp = new SanPhamShop();
                sp.Id = item.Id;
                sp.Ten = item.Ten;
                sp.GiaMoi = item.GiaMoi;
                sp.GiaCu = item.GiaCu;
                sp.Loai = item.Loai;
                sp.Anh = item.Anh;
                sp.DacSan = item.DacSan;

                list.Add(sp);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/shopapi/shopdata/searchproduct/{search}")]
        public IHttpActionResult SearchProduct(string search)
        {


            string[] timkiem = search.Split(' ');

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


            List<SanPhamShop> listSort = new List<SanPhamShop>();
            foreach (var item in outOK)
            {
                SanPhamShop sp = new SanPhamShop();
                sp.Id = item.Id;
                sp.Ten = item.Ten;
                sp.GiaMoi = item.GiaMoi;
                sp.GiaCu = item.GiaCu;
                sp.Loai = item.Loai;
                sp.Anh = item.Anh;
                sp.DacSan = item.DacSan;

                listSort.Add(sp);
            }


            return Ok(listSort);
        }



    }

    public class SanPhamAPI
    {

        public int Id { get; set; }
        public string Ten { get; set; }
        public Nullable<int> GiaMoi { get; set; }
        public Nullable<int> GiaCu { get; set; }
        public string MoTa { get; set; }
        public string Loai { get; set; }
        public Nullable<int> IdNguoiBan { get; set; }
        public string TenNguoiBan { get; set; }
        public string Anh { get; set; }
        public Nullable<int> LuotMua { get; set; }
        public Nullable<int> SoLuong { get; set; }
        public Nullable<bool> DacSan { get; set; }
        public string DacSanD1 { get; set; }
        public string DacSanD2 { get; set; }
        public Nullable<int> VaoVu { get; set; }
        public Nullable<int> RaVu { get; set; }
        public Nullable<int> GLD { get; set; }
        public Nullable<int> MNSL { get; set; }
        public Nullable<int> MNG { get; set; }
        public Nullable<int> TVSPLM { get; set; }
        public Nullable<int> TVM { get; set; }
        public Nullable<int> TVG { get; set; }
        public string DonViTinh { get; set; }
        public string DacSanD3 { get; set; }
        public string GoiY { get; set; }
        public string GhiChu { get; set; }

    }


    public class SanPhamShop{

        public int Id { get; set; }
        public string Ten { get; set; }
        public Nullable<int> GiaMoi { get; set; }
        public Nullable<int> GiaCu { get; set; }
        public string Loai { get; set; }
        public string Anh { get; set; }
        public Nullable<bool> DacSan { get; set; }
    }
}
