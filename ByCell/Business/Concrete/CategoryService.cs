using Business.Abstract;
using Business.Constants;
using Business.Security;
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
    //Standart CRUD işlemleri yapılır
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;

        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        public IResult Create(string name)
        {
            var category = _uow.Categories.Get(c => c.Name == name && c.IsActive==true);
            if (category!=null)
            {
                return new ErrorResult(Messages.ExistingCategory);
            }
            var newCategory = new Category
            {
                Name = name,
                IsActive = true
            };

            _uow.Categories.Add(newCategory);
            _uow.Commit();

            return new SuccessResult(Messages.CategoryAdded);
        }

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        public IResult Delete(int id)
        {
            var category = _uow.Categories.Get(c => c.Id == id && c.IsActive==true);
            if (category is null)
            {
                return new ErrorResult(Messages.CategoryAlreadyNotExist);
            }

            category.IsActive = false;

            _uow.Categories.Update(category);
            _uow.Commit();

            return new SuccessResult(Messages.CategoryRemoved);
        }

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        public IResult Edit(int id,string name)
        {
            var category = _uow.Categories.Get(c => c.Name == name && c.IsActive == true);

            category.Name = string.IsNullOrEmpty(name) ? category.Name : name;

            _uow.Categories.Update(category);
            _uow.Commit();

            return new SuccessResult(Messages.CategoryUpdated);

        }

        public IDataResult<List<Category>> GetAll()
        {
            var categories = _uow.Categories.GetAll(c=>c.IsActive==true).ToList();
            return new SuccessDataResult<List<Category>>(categories);
        }

        public IDataResult<Category> GetById(int id)
        {
            var category = _uow.Categories.Get(c => c.Id == id && c.IsActive==true);
            if (category is null)
            {
                return new ErrorDataResult<Category>(Messages.CategoryNotFound);
            }
            return new SuccessDataResult<Category>(category);
        }
    }
}
