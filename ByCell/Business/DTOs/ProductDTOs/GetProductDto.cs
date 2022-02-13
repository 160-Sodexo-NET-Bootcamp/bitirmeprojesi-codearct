using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.ProductDTOs
{
    public class GetProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public string ProductBrand { get; set; }
        public string UsageStatus { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public string IsOfferable { get; set; }
        public string IsSold { get; set; }
    }
}
