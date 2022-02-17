using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly ByCellDbContext _context;

        public IUserDal Users { get; private set; }
        public ICategoryDal Categories { get; private set; }
        public IColorDal Colors { get; private set; }
        public IProductBrandDal ProductBrands { get; private set; }
        public IUsageStatusDal UsageStatuses { get; private set; }
        public IProductDal Products { get; private set; }
        public IOfferDal Offers { get; private set; }
        public ISentMailDal SentMails { get; private set; }

        public EfUnitOfWork(ByCellDbContext context,
                            IUserDal users,
                            ICategoryDal categories,
                            IColorDal colors,
                            IProductBrandDal productBrands,
                            IUsageStatusDal usageStatuses,
                            IProductDal products,
                            IOfferDal offers, 
                            ISentMailDal sentMails)
        {
            _context = context;
            Users = users;
            Categories = categories;
            Colors = colors;
            ProductBrands = productBrands;
            UsageStatuses = usageStatuses;
            Products = products;
            Offers = offers;
            SentMails = sentMails;
        }
        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw new Exception("An error occured during saving!!!");
            }

        }
        public void Commit()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (Exception)
            {

                throw new Exception("An error occured during saving!!!");
            }

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
