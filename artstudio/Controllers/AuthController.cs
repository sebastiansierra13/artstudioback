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

        // M�todo de login
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
                return Unauthorized(new { success = false, message = "Contrase�a incorrecta" });
            }

            // Si la verificaci�n es correcta, contin�a con la l�gica de autenticaci�n.
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
            Response.Cookies.Delete("admin_session"); // Eliminamos la cookie de sesi�n
            return Ok(new { success = true, message = "Sesi�n cerrada" });
        }

        // M�todo para cambiar la contrase�a
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var adminInDb = _context.Admins.FirstOrDefault(a => a.User == changePasswordDto.User);
            if (adminInDb == null)
            {
                return NotFound("Admin not found");
            }

            // Verifica si la contrase�a actual es correcta
            if (adminInDb.Password != changePasswordDto.CurrentPassword)
            {
                return Unauthorized("Current password is incorrect");
            }

            // Hashea la nueva contrase�a
            adminInDb.Password = _passwordHasher.HashPassword(adminInDb, changePasswordDto.NewPassword);
            _context.SaveChanges();

            return Ok("Password changed successfully");
        }

        // M�todo para hashear contrase�as
        [HttpPost("hash-passwords")]
        public IActionResult HashPasswords()
        {
            var admins = _context.Admins.ToList();
            var passwordHasher = new PasswordHasher<Admin>();

            foreach (var admin in admins)
            {
                // Hashea la contrase�a y actualiza en la base de datos
                admin.Password = passwordHasher.HashPassword(admin, admin.Password);
            }

            _context.SaveChanges();
            return Ok("Passwords hashed successfully");
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

}
