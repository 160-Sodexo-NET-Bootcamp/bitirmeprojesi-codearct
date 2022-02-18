using Core.Entities.Concrete;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class ByCellDbContext : DbContext
    {
        public ByCellDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<UsageStatus> UsageStatuses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<SentMail> SentMails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Veritabanında Offers tablosunun OfferedPercentage kolonunun decimal türünün formunu belirliyor
            modelBuilder.Entity<Offer>().Property(c => c.OfferedPricePercentage).HasColumnType("decimal(5, 2)");
            //Veritabanında Offers tablosunun OfferedPrice kolonunun decimal türünün formunu belirliyor
            modelBuilder.Entity<Offer>().Property(c => c.OfferedPrice).HasColumnType("decimal(10, 2)");
            //Veritabanında Products tablosunun Price kolonunun decimal türünün formunu belirliyor
            modelBuilder.Entity<Product>().Property(c => c.Price).HasColumnType("decimal(10, 2)");
            base.OnModelCreating(modelBuilder);
        }
    }
}
