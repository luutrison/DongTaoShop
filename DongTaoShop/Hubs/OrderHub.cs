using DongTaoShop.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Newtonsoft.Json;
using System.Linq;

namespace DongTaoShop.Hubs
{
    [HubName("Order")]
    public class OrderHub : Hub
    {
        private readonly DongTaoShopEntities db = new DongTaoShopEntities();
        public void Login(string token)
        {
            System.Collections.Generic.List<TaiKhoan> a = db.TaiKhoans.Where(x => x.Token.Equals(token)).ToList();
            if (a.Any())
            {
                Groups.Add(Context.ConnectionId, "Order");
                Order();
            }
            else
            {
                Groups.Add(Context.ConnectionId, "Order");
                Clients.Group("Order").ok("None");
            }
        }
        public void Order()
        {
            var list = db.DonHangs.Select(p => new { p.Id, p.IdNguoiMua, p.IdSanPham, p.MuaLanDau, p.MuaNhieu, p.NgayDat, p.SoLuong, p.ThanhVien, p.TinhTrang, p.TongCong }).ToList();

            string json = JsonConvert.SerializeObject(list);
            Clients.Group("Order").ok(json);
        }
    }
}