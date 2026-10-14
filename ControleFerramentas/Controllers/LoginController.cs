using ControleFerramentas.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ControleFerramentas.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;
        private readonly AppDbContext _context;

        public LoginController(ILogger<LoginController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Emprestimo");
                }
                else
                {
                    return RedirectToAction("Index", "Usuario");
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string senha)
        {
            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(p => p.Email == email && p.Senha == senha);

            if (pessoa == null)
            {
                ViewBag.Erro = "Email ou senha inválidos!";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, pessoa.Id.ToString()),
                new Claim(ClaimTypes.Name, pessoa.Nome),
                new Claim(ClaimTypes.Role, pessoa.Perfil)
            };

            var identidade = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identidade);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (pessoa.Perfil == "Admin")
            {
                return RedirectToAction("Index", "Emprestimo");
            }
            else
            {
                return RedirectToAction("Index", "Usuario");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Sair()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}