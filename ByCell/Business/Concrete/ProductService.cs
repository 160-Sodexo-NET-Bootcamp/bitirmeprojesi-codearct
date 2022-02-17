using AutoMapper;
using Business.Abstract;
using Business.Constants;
using Business.DTOs.ProductDTOs;
using Business.Security;
using Business.ValidationRules.FluentValidation.ProductValidation;
using Core.Aspects.Autofac.Validation;
using Core.Entities.Concrete;
using Core.Results;
using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _uow = uow;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [SecuredOperation(Priority =1)]
        [ValidationAspect(typeof(CreateProductValidator))]
        public IResult Create(CreateProductDto createProductDto)
        {
            var product = _uow.Products.Get(p => p.Name == createProductDto.Name);
            if (product!=null)
            {
                return new ErrorResult(Messages.ExistingProduct);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            product = new Product
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                UserId = userId,
                CategoryId = createProductDto.CategoryId,
                UsageStatusId = createProductDto.UsageStatusId,
                ColorId = createProductDto.ColorId,
                ProductBrandId = createProductDto.BrandId,
                Price=createProductDto.Price,
                ImagePath = string.Empty,
                IsOfferable = false,
                IsSold = false,
                IsActive = true
            };
            

            _uow.Products.Add(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductAdded);

        }

        [SecuredOperation()]
        public IResult UploadProductImage(int id,string imagePath)
        {
            var product = _uow.Products.Get(c => c.Id == id 
                                         && c.IsSold == false
                                         && c.IsActive == true);
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId != userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            product.ImagePath = imagePath;

            _uow.Products.Update(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductImageUploaded);
        }

        [SecuredOperation()]
        public IResult Delete(int id)
        {
            
            var product = _uow.Products.Get(c => c.Id == id && c.IsActive == true);
            if (product is null)
            {
                return new ErrorResult(Messages.ProductAlreadyNotExist);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId!=userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            product.IsActive = false;

            _uow.Products.Update(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductRemoved);
        }

        [SecuredOperation(Priority = 1)]
        [ValidationAspect(typeof(UpdateProductValidator))]
        public IResult Edit(int id, UpdateProductDto updateProductDto)
        {
            var product = _uow.Products.Get(c => c.Id == id 
                                        && c.IsSold == false
                                        && c.IsActive == true);
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (product.UserId!=userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            product.Name = string.IsNullOrEmpty(updateProductDto.Name) ? product.Name : updateProductDto.Name;
            product.Description = string.IsNullOrEmpty(updateProductDto.Description) ? product.Description : updateProductDto.Description;
            product.CategoryId = updateProductDto.CategoryId == default ? product.CategoryId : updateProductDto.CategoryId;
            product.ColorId = updateProductDto.ColorId == default ? product.ColorId : updateProductDto.ColorId;
            product.ProductBrandId = updateProductDto.BrandId == default ? product.ProductBrandId : updateProductDto.BrandId;
            product.UsageStatusId = updateProductDto.UsageStatusId == default ? product.UsageStatusId : updateProductDto.UsageStatusId;
            product.IsOfferable = updateProductDto.IsOfferable == default 
                ? (product.IsOfferable ==true?false: product.IsOfferable)
                : updateProductDto.IsOfferable;

            _uow.Products.Update(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductUpdated);
        }

        public IDataResult<List<GetProductDto>> GetAllProducts()
        {
            var products = _uow.Products.GetAllProducts(p=>p.IsActive==true);
            var productDtos = _mapper.Map<List<GetProductDto>>(products);
            return new SuccessDataResult<List<GetProductDto>>(productDtos);

        }

        public IDataResult<List<GetProductDto>> GetAllByCategoryId(int categoryId)
        {
            var category = _uow.Categories.Get(c => c.Id == categoryId);
            if (category is null)
            {
                return new ErrorDataResult<List<GetProductDto>>(Messages.CategoryNotFound);
            }
            var products = _uow.Products.GetAllProducts(p=>p.CategoryId==categoryId && p.IsActive==true);
            var productDtos = _mapper.Map<List<GetProductDto>>(products);
            return new SuccessDataResult<List<GetProductDto>>(productDtos);
        }

        [SecuredOperation()]
        public IDataResult<List<GetProductDto>> GetAllByUserId()
        {
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _uow.Users.Get(u => u.Id == userId);
            if (user is null)
            {
                return new ErrorDataResult<List<GetProductDto>>(Messages.UserNotFound);
            }
            var products = _uow.Products.GetAllProducts(p => p.UserId == userId && p.IsActive==true);
            var productDtos = _mapper.Map<List<GetProductDto>>(products);
            return new SuccessDataResult<List<GetProductDto>>(productDtos);
        }

        public IDataResult<GetProductDto> GetById(int id)
        {
            var product = _uow.Products.GetProduct(p => p.Id == id && p.IsActive==true);
            if (product is null)
            {
                return new ErrorDataResult<GetProductDto>(Messages.ProductNotFound);
            }
            var productDto = _mapper.Map<GetProductDto>(product);
            return new SuccessDataResult<GetProductDto>(productDto);
        }

        [SecuredOperation()]
        public IResult BuyProduct(int id)
        {
            var product = _uow.Products.Get(p => p.Id == id 
                                        && p.IsSold == false
                                        && p.IsActive == true);
            if (product is null)
            {
                return new ErrorDataResult<GetProductDto>(Messages.ProductNotFound);
            }
            product.IsOfferable = false;
            product.IsSold = true;
            _uow.Products.Update(product);
            return new SuccessResult(Messages.ProductSold);
        }
    }
}
