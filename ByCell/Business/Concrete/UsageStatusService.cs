using Business.Abstract;
using Business.Constants;
using Core.Results;
using DataAccess.Abstract;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UsageStatusService:IUsageStatusService
    {
        private readonly IUnitOfWork _uow;

        public UsageStatusService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IResult Create(string name)
        {
            var usageStatus = _uow.UsageStatuses.Get(c => c.Name == name && c.IsActive == true);
            if (usageStatus != null)
            {
                return new ErrorResult(Messages.ExistingUsageStatus);
            }
            var newUsageStatus = new UsageStatus
            {
                Name = name,
                IsActive = true
            };

            _uow.UsageStatuses.Add(newUsageStatus);
            _uow.Commit();

            return new SuccessResult(Messages.UsageStatusAdded);
        }

        public IResult Delete(int id)
        {
            var usageStatus = _uow.UsageStatuses.Get(c => c.Id == id && c.IsActive == true);
            if (usageStatus is null)
            {
                return new ErrorResult(Messages.UsageStatusAlreadyNotExist);
            }

            usageStatus.IsActive = false;

            _uow.UsageStatuses.Update(usageStatus);
            _uow.Commit();

            return new SuccessResult(Messages.UsageStatusRemoved);
        }

        public IResult Edit(int id, string name)
        {
            var usageStatus = _uow.UsageStatuses.Get(c => c.Name == name && c.IsActive == true);

            usageStatus.Name = string.IsNullOrEmpty(name) ? usageStatus.Name : name;

            _uow.UsageStatuses.Update(usageStatus);
            _uow.Commit();

            return new SuccessResult(Messages.UsageStatusUpdated);

        }

        public IDataResult<List<UsageStatus>> GetAll()
        {
            var usageStatuss = _uow.UsageStatuses.GetAll(c => c.IsActive == true).ToList();
            return new SuccessDataResult<List<UsageStatus>>(usageStatuss);
        }

        public IDataResult<UsageStatus> GetById(int id)
        {
            var usageStatus = _uow.UsageStatuses.Get(c => c.Id == id && c.IsActive == true);
            if (usageStatus is null)
            {
                return new ErrorDataResult<UsageStatus>(Messages.UsageStatusNotFound);
            }
            return new SuccessDataResult<UsageStatus>(usageStatus);
        }
    }
}
