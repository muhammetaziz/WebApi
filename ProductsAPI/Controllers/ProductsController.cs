using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;

namespace ProductsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsContext _context;
        public ProductsController(ProductsContext context)
        {
            _context = context;
        }
        #region Tum Productları Listeleme

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();

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
        #region Id ye gore listeleme

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int? id)
        {
            var products = await _context.Products.ToListAsync();
            if (id == null)
            {
                return NotFound();
            }

            var p = _context.Products?.FirstOrDefault(i => i.ProductId == id);

            if (p == null)
            {
                return NotFound();
            }
            else { return Ok(p); }
        }

        #endregion
        #region Product ekleme

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id=entity.ProductId },entity);
        
        }
        #endregion
    }
}
