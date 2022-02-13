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
    public interface IProductDal:IRepository<Product>
    {
        List<Product> GetAllProducts(Expression<Func<Product, bool>> filter=null);
        Product GetProduct(Expression<Func<Product, bool>> filter);
    }
}
