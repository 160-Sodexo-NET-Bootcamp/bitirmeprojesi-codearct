using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.OfferDTOs
{
    public class GetOfferDto
    {
        public int Id { get; set; }
        public decimal OfferedPrice { get; set; }
        public decimal OfferedPricePercentage { get; set; }
        public string User { get; set; }
        public string Product { get; set; }
        public string ConfirmStatus { get; set; }
    }
}
