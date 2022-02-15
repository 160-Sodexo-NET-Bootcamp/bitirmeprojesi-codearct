using Business.DTOs.ProductDTOs;
using Core.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IProductService
    {
        IDataResult<List<GetProductDto>> GetAllProducts();
        IDataResult<List<GetProductDto>> GetAllByUserId();//UserId HttpContext den gelecek
        IDataResult<List<GetProductDto>> GetAllByCategoryId(int categoryId);
        IDataResult<GetProductDto> GetById(int id);
        IResult Create(string imagePath,CreateProductDto createProductDto);
        IResult Edit(int id,UpdateProductDto updateProductDto, string imagePath = null);
        IResult Delete(int id);
        IResult BuyProduct(int id);
    }
}
