using Microsoft.AspNetCore.Mvc;
using artstudio.Models;
using Microsoft.AspNetCore.Authorization;

namespace artstudio.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly MiDbContext _context;

        public AuthController(MiDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Admin admin)
        {
            if (ModelState.IsValid)
            {
                var adminInDb = _context.Admins
                                        .FirstOrDefault(a => a.User == admin.User && a.Password == admin.Password);

                if (adminInDb != null)
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false, // Cambiar a true en producción con HTTPS
                        Expires = DateTime.Now.AddHours(1),
                        SameSite = SameSiteMode.None // Permitir cookies entre orígenes
                    };

                    Response.Cookies.Append("admin_session", "true", cookieOptions);

                    return Ok(new { success = true, message = "Login exitoso" });
                }

                return Unauthorized(new { success = false, message = "Credenciales incorrectas" });
            }

            return BadRequest(ModelState);
        }

        [HttpGet("check-session")]
        [AllowAnonymous]
        public IActionResult CheckSession()
        {
            var session = Request.Cookies["admin_session"];
            if (session == "true")
            {
                return Ok(new { authenticated = true });
            }
            return Unauthorized(new { authenticated = false });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("admin_session");
            return Ok(new { success = true, message = "Sesión cerrada" });
        }
    }
}
