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
        public DbSet<OfferConfirm> OfferConfirms { get; set; }

    }
}
