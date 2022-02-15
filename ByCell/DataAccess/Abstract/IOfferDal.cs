using Core.DataAccess;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface IOfferDal : IRepository<Offer>
    {
        List<Offer> GetAllOffers(Expression<Func<Offer, bool>> filter = null);
        Offer GetOffer(Expression<Func<Offer, bool>> filter);
    }
    
}
