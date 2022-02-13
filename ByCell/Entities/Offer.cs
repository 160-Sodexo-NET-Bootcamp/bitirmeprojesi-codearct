using Core.Entities.Abstarct;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Offer : IEntity
    {
        public int Id { get ; set ; }
        public decimal OfferedPrice { get; set; }
        public decimal OfferedPricePercentage { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public short ConfirmStatus { get; set; }
        public bool IsActive { get; set; }

    }
}
