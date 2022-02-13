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
    public class EfProductDal:EfRepository<Product>,IProductDal
    {
        public EfProductDal(ByCellDbContext context):base(context)
        {

        }

        public List<Product> GetAllProducts(Expression<Func<Product, bool>> filter = null)
        {
            return filter==null 
                    ? _dbSet.Include(p => p.Category)
                                 .Include(p => p.Color)
                                 .Include(p => p.UsageStatus)
                                 .Include(p => p.ProductBrand)
                                 .Include(p => p.User)
                                 .OrderBy(p => p.Id)
                                 .ToList()
                    : _dbSet.Include(p => p.Category)
                                 .Include(p => p.Color)
                                 .Include(p => p.UsageStatus)
                                 .Include(p => p.ProductBrand)
                                 .Include(p => p.User)
                                 .Where(filter)
                                 .OrderBy(p => p.Id)
                                 .ToList();
        }

        public Product GetProduct(Expression<Func<Product, bool>> filter)
        {
            return _dbSet.Include(p => p.Category)
                                 .Include(p => p.Color)
                                 .Include(p => p.UsageStatus)
                                 .Include(p => p.ProductBrand)
                                 .Include(p => p.User)
                                 .Where(filter) as Product;
        }
    }
}
