
using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using artstudio.DTOs;


namespace artstudio.Controllers
{
    [Route("api/productos")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly MiDbContext _context;

        public ProductoController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.Include(p => p.IdCategoriaNavigation).ToListAsync();
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
