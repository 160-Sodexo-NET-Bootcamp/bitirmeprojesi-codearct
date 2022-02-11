using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfProductBrandDal:EfRepository<ProductBrand>,IProductBrandDal
    {
        public EfProductBrandDal(ByCellDbContext context):base(context)
        {

        }
    }
}
