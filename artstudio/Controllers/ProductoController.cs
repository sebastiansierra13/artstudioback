
using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using artstudio.DTOs;
using artstudio.Services;


namespace artstudio.Controllers
{
    [Route("api/productos")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly MiDbContext _context;
        private readonly ProductService _productService;
        public ProductoController(MiDbContext context, ProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.Include(p => p.IdCategoriaNavigation).ToListAsync();
        }

        [HttpGet("ultimos")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetUltimosProductos()
        {
            var ultimosProductos = await _context.Productos
                .OrderByDescending(p => p.IdProducto) // Asumiendo que IdProducto es auto-incremental
                .Take(9)
                .Include(p => p.IdCategoriaNavigation)
                .ToListAsync();

            return Ok(ultimosProductos);
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.Include(p => p.IdCategoriaNavigation).FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound();
            }

            return producto;
        }

        [HttpGet("destacados")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetDestacados()
        {
            var destacados = await _context.Productos
                                           .Where(p => p.Destacado == true)
                                           .OrderBy(p => p.Posicion)
                                           .ToListAsync();
            return destacados;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Producto>>> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return BadRequest("La consulta de búsqueda no puede estar vacía.");
            }

            var results = await _productService.SearchProductsAsync(query);
            return Ok(results);
        }


        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            if (producto == null)
            {
                return BadRequest("El producto no puede ser nulo");
            }

            if (string.IsNullOrEmpty(producto.NombreProducto))
            {
                return BadRequest("El nombre del producto es requerido");
            }

            // Verificar si el ID ya existe para evitar duplicados
            var existingProduct = await _context.Productos.FindAsync(producto.IdProducto);
            if (existingProduct != null)
            {
                return Conflict("El ID del producto ya existe. Intente de nuevo.");
            }

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducto), new { id = producto.IdProducto }, producto);
        }


        // GET: api/productos/categoria/5
        [HttpGet("categoria/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosByCategoria(int idCategoria)
        {
            var productos = await _context.Productos
                .Where(p => p.IdCategoria == idCategoria)
                .ToListAsync();

            if (productos == null || !productos.Any())
            {
                return Ok(new List<Producto>()); // Devolver una lista vacía en lugar de NotFound
            }

            return Ok(productos);
        }

        public class UpdateProductDto
        {
            public int IdProducto { get; set; }
            public bool? Destacado { get; set; }
            public int? Posicion { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, UpdateProductDto updateProductDto)
        {
            if (id != updateProductDto.IdProducto)
            {
                return BadRequest("El ID del producto no coincide.");
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Solo actualizamos los campos necesarios
            producto.Destacado = updateProductDto.Destacado ?? producto.Destacado;
            producto.Posicion = updateProductDto.Posicion ?? producto.Posicion;

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error actualizando el producto: {ex.Message}");
                return BadRequest("Error actualizando el producto.");
            }

            return NoContent();
        }


        [HttpPut("update-positions")]
        public async Task<IActionResult> UpdateProductPositions([FromBody] List<UpdateProductPositionDto> updateProductPositions)
        {
            foreach (var update in updateProductPositions)
            {
                var producto = await _context.Productos.FindAsync(update.IdProducto);
                if (producto != null)
                {
                    producto.Posicion = update.Posicion;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Error actualizando las posiciones");
            }

            return NoContent();
        }

        [HttpGet("{id}/relacionados")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductosRelacionados(int id)
        {
            try
            {
                // Obtener el producto actual
                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.IdProducto == id);

                if (producto == null)
                {
                    Console.WriteLine($"Producto con ID {id} no encontrado.");
                    return NotFound($"No se encontró el producto con ID {id}");
                }

                Console.WriteLine($"Producto encontrado: ID {producto.IdProducto}, Nombre: {producto.NombreProducto}");

                // Obtener los IDs de los tags del producto actual
                var tagIds = new List<int>();
                if (!string.IsNullOrEmpty(producto.ListTags))
                {
                    tagIds = producto.ListTags.Split(',')
                                              .Select(s => int.TryParse(s.Trim(), out var result) ? result : (int?)null)
                                              .Where(i => i.HasValue)
                                              .Select(i => i.Value)
                                              .ToList();
                }

                Console.WriteLine($"Tags del producto: {string.Join(", ", tagIds)}");

                if (!tagIds.Any())
                {
                    Console.WriteLine("El producto no tiene tags asociados.");
                    return Ok(new List<Producto>());
                }

                // Buscar productos relacionados (que no sean el producto actual)
                var productosRelacionados = await _context.Productos
                    .ToListAsync(); // Obtén todos los productos primero

                var productosRelacionadosFiltrados = productosRelacionados
                    .Where(p => p.IdProducto != id &&
                                tagIds.Any(t => !string.IsNullOrEmpty(p.ListTags) && p.ListTags.Split(',').Contains(t.ToString())))
                    .ToList();

                Console.WriteLine($"Productos relacionados encontrados: {productosRelacionadosFiltrados.Count}");

                // Imprimir detalles de los productos relacionados
                foreach (var prod in productosRelacionadosFiltrados)
                {
                    Console.WriteLine($"Producto relacionado: ID {prod.IdProducto}, Nombre: {prod.NombreProducto}");
                    Console.WriteLine($"Tags: {prod.ListTags}");
                }

                return Ok(productosRelacionadosFiltrados);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetProductosRelacionados: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, "Ocurrió un error interno del servidor. Por favor, revisa los logs para más detalles.");
            }
        }



        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }
    }



}
