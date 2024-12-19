using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.DTO;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsContext _context;
        public ProductsController(ProductsContext context) { _context = context; }

        #region Product List All
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.Where(i => i.IsActive).Select(p => productToDTO(p)).ToListAsync();

            if (_context == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(products);
            }
        }
        #endregion
        #region Product Select With ID

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);

            if (p == null)
            {
                return NotFound();
            }
            var productDto = productToDTO(p);
            return Ok(productDto);
        }

        #endregion
        #region Add Product

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = entity.ProductId }, entity);

        }
        #endregion
        #region Update Product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product entity)
        {
            if (id != entity.ProductId)
            {
                return BadRequest();
            }

            var p = await _context.Products.FirstOrDefaultAsync(i => i.ProductId == id);
            if (p == null)
            {
                return NotFound();
            }
            p.ProductName = entity.ProductName;
            p.Price = entity.Price;
            p.IsActive = entity.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return NoContent();


        }
        #endregion
        #region Delete Product

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (p == null)
            {
                return NotFound();
            }

            _context.Products.Remove(p);
            try
            {
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                return BadRequest();
            }
            return NoContent();
        }
        #endregion
        
        private static ProductDTO productToDTO(Product p) { return new ProductDTO { ProductName = p.ProductName, Price = p.Price, ProductId = p.ProductId }; }
    }
}
