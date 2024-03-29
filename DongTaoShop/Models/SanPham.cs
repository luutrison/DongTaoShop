//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DongTaoShop.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SanPham()
        {
            this.BinhLuans = new HashSet<BinhLuan>();
            this.DonHangs = new HashSet<DonHang>();
        }
    
        public int Id { get; set; }
        public string Ten { get; set; }
        public Nullable<int> GiaMoi { get; set; }
        public Nullable<int> GiaCu { get; set; }
        public string MoTa { get; set; }
        public string Loai { get; set; }
        public Nullable<int> IdNguoiBan { get; set; }
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
        public string TimKiem { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DonHang> DonHangs { get; set; }
        public virtual TaiKhoan TaiKhoan { get; set; }
    }
}
