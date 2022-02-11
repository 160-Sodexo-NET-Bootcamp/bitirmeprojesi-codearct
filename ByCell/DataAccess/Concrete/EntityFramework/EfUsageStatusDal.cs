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
    public class EfUsageStatusDal:EfRepository<UsageStatus>,IUsageStatusDal
    {
        public EfUsageStatusDal(ByCellDbContext context):base(context)
        {

        }
    }
}
