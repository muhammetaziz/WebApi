using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProductsAPI.Models
{
    public class ProductsContext : IdentityDbContext<AppUser,AppRole,int>
    {
        public ProductsContext(DbContextOptions<ProductsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 1, ProductName = "Samsung", Price = 15890, IsActive = true });
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 2, ProductName = "IPhone", Price = 21222, IsActive = true });
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 3, ProductName = "Redmi", Price = 41890, IsActive = true });
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 4, ProductName = "Vestel", Price = 123890, IsActive = false });
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 5, ProductName = "Xiomi", Price = 51222, IsActive = true });
            modelBuilder.Entity<Product>().HasData(new Product() { ProductId = 6, ProductName = "SonyEricson", Price = 132, IsActive = false });
        }
        public DbSet<Product> Products { get; set; }

    }
}
