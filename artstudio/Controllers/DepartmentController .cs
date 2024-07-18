using artstudio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace artstudio.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly MiDbContext _context;

        public DepartmentController(MiDbContext context)
        {
            _context = context;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Departamento>>> GetDepartments()
        {
            return await _context.Departamentos.ToListAsync();
        }

        // GET: api/departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Departamento>> GetDepartment(int id)
        {
            var department = await _context.Departamentos.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // POST: api/departments
        [HttpPost]
        public async Task<ActionResult<Departamento>> PostDepartment(Departamento department)
        {
            _context.Departamentos.Add(department);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartment), new { id = department.Id }, department);
        }

        // PUT: api/departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Departamento department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }

            _context.Entry(department).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _context.Departamentos.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departamentos.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departamentos.Any(e => e.Id == id);
        }
    }
}
