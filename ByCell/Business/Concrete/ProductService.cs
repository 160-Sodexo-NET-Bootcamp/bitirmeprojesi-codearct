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

        //Yetki kontrolü yapılır
        [SecuredOperation(Priority =1)]
        //Validasyon kontrolü yapılır
        [ValidationAspect(typeof(CreateProductValidator))]
        public IResult Create(CreateProductDto createProductDto)
        {
            //Verilen ürün ismine göre veritabanında arama yapılır
            var product = _uow.Products.Get(p => p.Name == createProductDto.Name && p.IsActive==true);
            //Ürün varsa ilgili hata mesajı kullanıcıya dönülür
            if (product!=null)
            {
                return new ErrorResult(Messages.ExistingProduct);
            }
            //Verilen Dto ve isteği yapan kullanıcının token ile gelen Id bilgisiyle ürün oluşturulur
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

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        //Ürüne ait fotoğraf eklenir
        public IResult UploadProductImage(int id,string imagePath)
        {
            //Verilen Id ye göre satışı yapılmamış ve silinmemiş ürünler arasında arama yapılır
            var product = _uow.Products.Get(c => c.Id == id 
                                         && c.IsSold == false
                                         && c.IsActive == true);
            //Ürün yoksa ilgili hata mesajı kullanıcıya dönülür
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            //Ürün varsa ürünün sahibi isteği yapan kullanıcı mı kontrolü yapılır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Ürün sahibi isteği yapan kullanıcı değilse ilgili hata mesajı dönülür
            if (product.UserId != userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            //Controllerdan gelen fotoğraf dosya yoluna göre ilgili ürün veritababnında güncellenir
            product.ImagePath = imagePath;

            _uow.Products.Update(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductImageUploaded);
        }

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        //Ürün soft olarak veritabanından silinir
        public IResult Delete(int id)
        {
            //verilen Id ye göre silinmemiş ürünler arasından arama yapılır
            var product = _uow.Products.Get(c => c.Id == id && c.IsActive == true);
            //Ürün zaten yoksa ilgili hata mesajı kullanıcıya dönülür
            if (product is null)
            {
                return new ErrorResult(Messages.ProductAlreadyNotExist);
            }

            //Ürün varsa ürünün sahibi isteği yapan kullanıcı mı kontrolü yapılır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //İsteği yapan kullanıcı ürün sahibi değilse ilgili hata mesajı kullanıcıya dönülür
            if (product.UserId!=userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            //İş kurallarından geçilirse ürün soft olarak veritabanından silinir
            product.IsActive = false;

            _uow.Products.Update(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductRemoved);
        }

        //Yetki kontrolü yapılır
        [SecuredOperation(Priority = 1)]
        //Validasyon kontrolü yapılır
        [ValidationAspect(typeof(UpdateProductValidator))]
        //Ürün güncellenir
        public IResult Edit(int id, UpdateProductDto updateProductDto)
        {
            //Verilen ürün Id sine göre satılmamış ve silinmemiş ürünler arasında arama yapılır
            var product = _uow.Products.Get(c => c.Id == id 
                                        && c.IsSold == false
                                        && c.IsActive == true);
            //Ürün yoksa ilgili hata mesajı dönülür
            if (product is null)
            {
                return new ErrorResult(Messages.ProductNotFound);
            }

            //Ürün sahibi istek sahibi mi kontrol edilir
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Ürün istek sahibine ait değilse ilgili hata mesajı dönülür
            if (product.UserId!=userId)
            {
                return new ErrorResult(Messages.ProductAuthDenied);
            }

            //İş kurallarından geçilirse Dto dan gelen veriler ile ürün güncellenir
            product.Name = string.IsNullOrEmpty(updateProductDto.Name) 
                ? product.Name 
                : updateProductDto.Name;
            product.Description = string.IsNullOrEmpty(updateProductDto.Description) 
                ? product.Description 
                : updateProductDto.Description;
            product.CategoryId = updateProductDto.CategoryId == default 
                ? product.CategoryId 
                : updateProductDto.CategoryId;
            product.ColorId = updateProductDto.ColorId == default 
                ? product.ColorId 
                : updateProductDto.ColorId;
            product.ProductBrandId = updateProductDto.BrandId == default 
                ? product.ProductBrandId 
                : updateProductDto.BrandId;
            product.UsageStatusId = updateProductDto.UsageStatusId == default 
                ? product.UsageStatusId 
                : updateProductDto.UsageStatusId;
            product.IsOfferable = updateProductDto.IsOfferable == default 
                ? (product.IsOfferable ==true?false: product.IsOfferable)
                : updateProductDto.IsOfferable;

            _uow.Products.Update(product);
            _uow.Commit();

            return new SuccessResult(Messages.ProductUpdated);
        }

        //Bütün ürünler listelenir
        public IDataResult<List<GetProductDto>> GetAllProducts()
        {
            //Silinmemiş bütün ürünler veritabanından getirilir
            var products = _uow.Products.GetAllProducts(p=>p.IsActive==true);
            //Automapper ile kullanıcıya gösterilecek formata dönüştürülür
            var productDtos = _mapper.Map<List<GetProductDto>>(products);
            //Formatlanmış ürün bilgileri kullanıcıya dönülür
            return new SuccessDataResult<List<GetProductDto>>(productDtos);

        }

        //Kategoriye göre bütün ürünler listelenir
        public IDataResult<List<GetProductDto>> GetAllByCategoryId(int categoryId)
        {
            //Verilen kategori Id sine göre kategori veritabanında aranır
            var category = _uow.Categories.Get(c => c.Id == categoryId);
            //Kategori yoksa ilgili hata mesajı kullanıcıya dönülür
            if (category is null)
            {
                return new ErrorDataResult<List<GetProductDto>>(Messages.CategoryNotFound);
            }
            //Verilen kategori Id ye göre silinmemiş bütün ürünler getirilir
            var products = _uow.Products.GetAllProducts(p=>p.CategoryId==categoryId && p.IsActive==true);
            //Automapper ile kullanıcıya gösterilecek formata dönüştürülür
            var productDtos = _mapper.Map<List<GetProductDto>>(products);
            //Formatlanan bu veri kullanıcıya dönülür
            return new SuccessDataResult<List<GetProductDto>>(productDtos);
        }

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        //Kullanıcı kendine ait bütün ürünleri listeler
        public IDataResult<List<GetProductDto>> GetAllByUserId()
        {
            //İsteği yapan kullanıcının Id si token üzerinden alınır
            var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            //Bu kullanıcı Id sine göre silinmemiş bütün ürünler getirlir
            var products = _uow.Products.GetAllProducts(p => p.UserId == userId && p.IsActive==true);
            //Automapper ile kullanıcıya gösterilecek şekilde formatlanır
            var productDtos = _mapper.Map<List<GetProductDto>>(products);
            //Formatlanmış veri kullanıcıya dönülür
            return new SuccessDataResult<List<GetProductDto>>(productDtos);
        }

        //Ürün detayları gösterilir
        public IDataResult<GetProductDto> GetById(int id)
        {
            //Verilen Id ye göre silinmemiş ürünler arasında aranır
            var product = _uow.Products.GetProduct(p => p.Id == id && p.IsActive==true);
            //Ürün yoksa ilgili hata mesajı kullanıcıya dönülür
            if (product is null)
            {
                return new ErrorDataResult<GetProductDto>(Messages.ProductNotFound);
            }
            //Bulunan ürün kullanıcıya formatlanarak gösterilir
            var productDto = _mapper.Map<GetProductDto>(product);
            return new SuccessDataResult<GetProductDto>(productDto);
        }

        //Yetki kontrolü yapılır
        [SecuredOperation()]
        //Ürün satın alınır
        public IResult BuyProduct(int id)
        {
            //Verilen Id ye göre satılmamış ve silinmemiş ürünler arasında aranır
            var product = _uow.Products.Get(p => p.Id == id 
                                        && p.IsSold == false
                                        && p.IsActive == true);
            //Ürün yoksa ilgili hata mesajı kullanıcıya gösterilir
            if (product is null)
            {
                return new ErrorDataResult<GetProductDto>(Messages.ProductNotFound);
            }
            //Ürünün teklife kapalı ve satıldı olarak güncellenir
            product.IsOfferable = false;
            product.IsSold = true;
            _uow.Products.Update(product);
            return new SuccessResult(Messages.ProductSold);
        }
    }
}
