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

        public EfUnitOfWork(ByCellDbContext context, IUserDal users)
        {
            _context = context;
            Users = users;
        }
        public async Task<bool> CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
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
