using artstudio.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace artstudio.Services
{
    public class ProductService
    {
        private readonly MiDbContext _context;

        public ProductService(MiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> SearchProductsAsync(string query)
        {
            var lowerQuery = query.ToLower();

            // Obtener todos los productos que podrían coincidir con el nombre o la categoría
            var products = await _context.Productos
                .Include(p => p.IdCategoriaNavigation)
                .Where(p => p.NombreProducto.ToLower().Contains(lowerQuery) ||
                            p.IdCategoriaNavigation.NombreCategoria.ToLower().Contains(lowerQuery) ||
                            !string.IsNullOrEmpty(p.ListTags) ||
                            !string.IsNullOrEmpty(p.DescripcionProducto))
                .ToListAsync();

            // Filtrar en memoria por tags y descripción
            var filteredProducts = products.Where(p =>
                p.NombreProducto.ToLower().Contains(lowerQuery) ||
                p.IdCategoriaNavigation.NombreCategoria.ToLower().Contains(lowerQuery) ||
                (p.ListTags?.Split(',').Any(tag => tag.Trim().ToLower().Contains(lowerQuery)) ?? false) ||
                (p.DescripcionProducto?.ToLower().Contains(lowerQuery) ?? false))
                .ToList();

            return filteredProducts;
        }

    }
}
