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
    public class OfferConfirmService:IOfferConfirmService
    {
        private readonly IUnitOfWork _uow;

        public OfferConfirmService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IResult Create(string name)
        {
            var offerConfirm = _uow.OfferConfirms.Get(c => c.Name == name && c.IsActive == true);
            if (offerConfirm != null)
            {
                return new ErrorResult(Messages.ExistingOfferConfirm);
            }
            var newOfferConfirm = new OfferConfirm
            {
                Name = name,
                IsActive = true
            };

            _uow.OfferConfirms.Add(newOfferConfirm);
            _uow.Commit();

            return new SuccessResult(Messages.OfferConfirmAdded);
        }

        public IResult Delete(int id)
        {
            var offerConfirm = _uow.OfferConfirms.Get(c => c.Id == id && c.IsActive == true);
            if (offerConfirm is null)
            {
                return new ErrorResult(Messages.OfferConfirmAlreadyNotExist);
            }

            offerConfirm.IsActive = false;

            _uow.OfferConfirms.Update(offerConfirm);
            _uow.Commit();

            return new SuccessResult(Messages.OfferConfirmRemoved);
        }

        public IResult Edit(int id, string name)
        {
            var offerConfirm = _uow.OfferConfirms.Get(c => c.Name == name && c.IsActive == true);

            offerConfirm.Name = string.IsNullOrEmpty(name) ? offerConfirm.Name : name;

            _uow.OfferConfirms.Update(offerConfirm);
            _uow.Commit();

            return new SuccessResult(Messages.OfferConfirmUpdated);

        }

        public IDataResult<List<OfferConfirm>> GetAll()
        {
            var offerConfirms = _uow.OfferConfirms.GetAll(c => c.IsActive == true).ToList();
            return new SuccessDataResult<List<OfferConfirm>>(offerConfirms);
        }

        public IDataResult<OfferConfirm> GetById(int id)
        {
            var offerConfirm = _uow.OfferConfirms.Get(c => c.Id == id && c.IsActive == true);
            if (offerConfirm is null)
            {
                return new ErrorDataResult<OfferConfirm>(Messages.OfferConfirmNotFound);
            }
            return new SuccessDataResult<OfferConfirm>(offerConfirm);
        }
    }
}
