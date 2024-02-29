using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace AuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : CustomBaseController
    {
        private readonly IGenericService<Product, ProductDto> _productService;
        public ProductController(IGenericService<Product, ProductDto> productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts() 
        {
            return ActionResultInstance(await _productService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDto dto)
        {
            return ActionResultInstance(await _productService.AddAsync(dto));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDto dto)
        {
            return ActionResultInstance(await _productService.Updade(dto,dto.Id));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            return ActionResultInstance(await _productService.Remove(id));
        }
    }
}
