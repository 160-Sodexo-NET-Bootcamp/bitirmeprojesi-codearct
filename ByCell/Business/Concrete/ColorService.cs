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
    public class ColorService:IColorService
    {
        private readonly IUnitOfWork _uow;

        public ColorService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [SecuredOperation()]
        public IResult Create(string name)
        {
            var color = _uow.Colors.Get(c => c.Name == name && c.IsActive == true);
            if (color != null)
            {
                return new ErrorResult(Messages.ExistingColor);
            }
            var newColor = new Color
            {
                Name = name,
                IsActive = true
            };

            _uow.Colors.Add(newColor);
            _uow.Commit();

            return new SuccessResult(Messages.ColorAdded);
        }

        [SecuredOperation()]
        public IResult Delete(int id)
        {
            var color = _uow.Colors.Get(c => c.Id == id && c.IsActive == true);
            if (color is null)
            {
                return new ErrorResult(Messages.ColorAlreadyNotExist);
            }

            color.IsActive = false;

            _uow.Colors.Update(color);
            _uow.Commit();

            return new SuccessResult(Messages.ColorRemoved);
        }

        [SecuredOperation()]
        public IResult Edit(int id, string name)
        {
            var color = _uow.Colors.Get(c => c.Name == name && c.IsActive == true);

            color.Name = string.IsNullOrEmpty(name) ? color.Name : name;

            _uow.Colors.Update(color);
            _uow.Commit();

            return new SuccessResult(Messages.ColorUpdated);

        }

        public IDataResult<List<Color>> GetAll()
        {
            var colors = _uow.Colors.GetAll(c => c.IsActive == true).ToList();
            return new SuccessDataResult<List<Color>>(colors);
        }

        public IDataResult<Color> GetById(int id)
        {
            var color = _uow.Colors.Get(c => c.Id == id && c.IsActive == true);
            if (color is null)
            {
                return new ErrorDataResult<Color>(Messages.ColorNotFound);
            }
            return new SuccessDataResult<Color>(color);
        }
    }
}
