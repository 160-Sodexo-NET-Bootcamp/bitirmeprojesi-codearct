using Business.Abstract;
using Business.DTOs.ProductDTOs;
using Core.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Route("products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public static IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductService productService, IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var result = _productService.GetAllProducts();
            return Ok(result);
        }

        [HttpGet("user/current")]
        public IActionResult GetAllProductsByUserId()
        {
            var result = _productService.GetAllByUserId();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _productService.GetById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public IActionResult GetAllByCategoryId(int categoryId)
        {
            var result = _productService.GetAllByCategoryId(categoryId);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateProductDto createProductDto)
        {
            var result = _productService.Create(createProductDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{productId}/image")]
        public IActionResult UploadProductImage(int productId,IFormFile ImageFile)
        {
            if (ImageFile.Length>0 &&(ImageFile.ContentType=="image/png" 
                                   || ImageFile.ContentType == "image/jpg" 
                                   || ImageFile.ContentType == "image/jpeg"))
            {
                string path = _webHostEnvironment.WebRootPath + "\\Images\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                using (FileStream stream=System.IO.File.Create(path+ImageFile.FileName))
                {
                    ImageFile.CopyTo(stream);
                    stream.Flush();
                }
                var result = _productService.UploadProductImage(productId, path + ImageFile.FileName);
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            else
            {
                return BadRequest("Bir resim dosyası yükleyin!");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Edit(int id,[FromBody] UpdateProductDto updateProductDto)
        {
            var result = _productService.Edit(id, updateProductDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{id}/buy")]
        public IActionResult BuyProduct(int id)
        {
            var result = _productService.BuyProduct(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _productService.Delete(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

    }
}
