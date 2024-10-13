using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using artstudio.Models;
using Microsoft.AspNetCore.Authorization;
namespace artstudio.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MiDbContext _context;
        private readonly PasswordHasher<Admin> _passwordHasher;

        public AuthController(MiDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Admin>();  // Usamos PasswordHasher para el hashing
        }

        // Método de login
        [HttpPost("login")]
        public IActionResult Login([FromBody] AdminLoginDto adminDto)
        {
            var adminInDb = _context.Admins.FirstOrDefault(a => a.User == adminDto.User);

            if (adminInDb == null)
            {
                return Unauthorized(new { success = false, message = "Usuario no encontrado" });
            }

            // Usa PasswordHasher para verificar el hash
            var passwordHasher = new PasswordHasher<object>();
            var result = passwordHasher.VerifyHashedPassword(null, adminInDb.Password, adminDto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized(new { success = false, message = "Contraseña incorrecta" });
            }

            // Si la verificación es correcta, continúa con la lógica de autenticación.
            return Ok(new { success = true, message = "Login exitoso" });
        }



        [HttpGet("check-session")]
        [AllowAnonymous]
        public IActionResult CheckSession()
        {
            if (Request.Cookies.TryGetValue("admin_session", out var session) && session == "true")
            {
                return Ok(new { authenticated = true });
            }
            return Unauthorized(new { authenticated = false });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("admin_session"); // Eliminamos la cookie de sesión
            return Ok(new { success = true, message = "Sesión cerrada" });
        }

        [HttpPut("update-password")]
        public IActionResult UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            try
            {
                // Buscar al admin en la base de datos
                var adminInDb = _context.Admins.FirstOrDefault(a => a.User == request.User);
                if (adminInDb != null)
                {
                    var passwordHasher = new PasswordHasher<Admin>();

                    // Hashear la nueva contraseña
                    adminInDb.Password = passwordHasher.HashPassword(adminInDb, request.NewPassword);

                    // Guardar los cambios
                    _context.SaveChanges();

                    return Ok("Password updated successfully");
                }
                else
                {
                    return NotFound("Admin not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

    }

    // DTOs (Data Transfer Objects)
    public class AdminLoginDto
    {
        public string User { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class ChangePasswordDto
    {
        public string User { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }

    public class UpdatePasswordRequest
    {
        public string User { get; set; }
        public string NewPassword { get; set; }
    }
}
