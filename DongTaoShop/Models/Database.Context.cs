﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DongTaoShopEntities : DbContext
    {
        public DongTaoShopEntities()
            : base("name=DongTaoShopEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }
    }
}
