using System.Web.Mvc;

namespace DongTaoShop.Controllers
{
    public class DataController : Controller
    {
        public static string LoaiMatHang(int? value)
        {
            if (value == 0)
            {
                return "Rau củ";
            }

            if (value == 1)
            {
                return "Thịt";
            }

            if (value == 2)
            {
                return "Thủy - Hải sản";
            }

            if (value == 3)
            {
                return "Hoa quả";
            }

            if (value == 4)
            {
                return "Nấm";
            }

            return null;
        }
        public static string DonViTinh(int? value)
        {
            if (value == 0)
            {
                return "KG";
            }

            if (value == 1)
            {
                return "CON";
            }

            if (value == 2)
            {
                return "QUẢ";
            }

            if (value == 3)
            {
                return "TÚI";
            }

            return null;
        }
        public static string GioiTinh(int? value)
        {
            if (value == 0)
            {
                return "NAM";
            }

            if (value == 1)
            {
                return "NỮ";
            }

            return null;
        }
        public static int? ToNullableInt(string s)
        {
            if (int.TryParse(s, out int i))
            {
                return i;
            }

            return null;
        }
    }
}