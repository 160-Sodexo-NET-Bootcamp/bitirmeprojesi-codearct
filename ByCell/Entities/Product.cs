using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Product : BaseEntity
    {
        public string Description { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public int ProductBrandId { get; set; }
        public ProductBrand ProductBrand { get; set; }
        public int UsageStatusId { get; set; }
        public UsageStatus UsageStatus { get; set; }
        public string  ImagePath { get; set; }
        public decimal Price { get; set; }
        public bool IsOfferable { get; set; }
        public bool IsSold { get; set; }
    }
}
