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
    public class ProductBrandService:IProductBrandService
    {
        private readonly IUnitOfWork _uow;

        public ProductBrandService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public IResult Create(string name)
        {
            var productBrand = _uow.ProductBrands.Get(c => c.Name == name && c.IsActive == true);
            if (productBrand != null)
            {
                return new ErrorResult(Messages.ExistingProductBrand);
            }
            var newProductBrand = new ProductBrand
            {
                Name = name,
                IsActive = true
            };

            _uow.ProductBrands.Add(newProductBrand);
            _uow.Commit();

            return new SuccessResult(Messages.ProductBrandAdded);
        }

        public IResult Delete(int id)
        {
            var productBrand = _uow.ProductBrands.Get(c => c.Id == id && c.IsActive == true);
            if (productBrand is null)
            {
                return new ErrorResult(Messages.ProductBrandAlreadyNotExist);
            }

            productBrand.IsActive = false;

            _uow.ProductBrands.Update(productBrand);
            _uow.Commit();

            return new SuccessResult(Messages.ProductBrandRemoved);
        }

        public IResult Edit(int id, string name)
        {
            var productBrand = _uow.ProductBrands.Get(c => c.Name == name && c.IsActive == true);

            productBrand.Name = string.IsNullOrEmpty(name) ? productBrand.Name : name;

            _uow.ProductBrands.Update(productBrand);
            _uow.Commit();

            return new SuccessResult(Messages.ProductBrandUpdated);

        }

        public IDataResult<List<ProductBrand>> GetAll()
        {
            var productBrands = _uow.ProductBrands.GetAll(c => c.IsActive == true).ToList();
            return new SuccessDataResult<List<ProductBrand>>(productBrands);
        }

        public IDataResult<ProductBrand> GetById(int id)
        {
            var productBrand = _uow.ProductBrands.Get(c => c.Id == id && c.IsActive == true);
            if (productBrand is null)
            {
                return new ErrorDataResult<ProductBrand>(Messages.ProductBrandNotFound);
            }
            return new SuccessDataResult<ProductBrand>(productBrand);
        }
    }
}
