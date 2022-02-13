using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.ProductDTOs
{
    public class UpdateProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int ColorId { get; set; }
        public int BrandId { get; set; }
        public int UsageStatusId { get; set; }
        public string ImagePath { get; set; }
        public decimal Price { get; set; }
        public bool IsOfferable { get; set; }
    }
}
