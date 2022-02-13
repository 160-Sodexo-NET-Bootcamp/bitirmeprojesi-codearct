using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTOs.OfferDTOs
{
    public class SendOfferDto
    {
        public decimal OfferedPrice { get; set; }
        public decimal OfferedPricePercentage { get; set; }
    }
}
