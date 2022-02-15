using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfOfferDal : EfRepository<Offer>, IOfferDal
    {
        public EfOfferDal(ByCellDbContext context) : base(context)
        {

        }
        public List<Offer> GetAllOffers(Expression<Func<Offer, bool>> filter = null)
        {
            return filter == null
                    ? _dbSet.Include(o => o.Product)
                                 .Include(o => o.User)
                                 .OrderBy(o => o.Id)
                                 .ToList()
                    : _dbSet.Include(o => o.Product)
                                 .Include(o => o.User)
                                 .Where(filter)
                                 .OrderBy(o => o.Id)
                                 .ToList();
        }

        public Offer GetOffer(Expression<Func<Offer, bool>> filter)
        {
            return _dbSet.Include(o => o.Product)
                                 .Include(o => o.User)
                                 .Where(filter) as Offer;
        }
    }
}
