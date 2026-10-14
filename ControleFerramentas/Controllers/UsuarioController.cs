using ControleFerramentas.Data;
using ControleFerramentas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ControleFerramentas.Controllers
{
    [Authorize(Roles = "Usuario")]
    public class UsuarioController : Controller
    {

        private readonly ILogger<UsuarioController> _logger;
        private readonly AppDbContext _context;
        public UsuarioController(ILogger<UsuarioController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string pesquisa, string status, int? dia, int? mes, int? ano, int pagina = 1)
        {
            int idUsuarioLogado = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var consulta = _context.Emprestimos
                .Include(e => e.Ferramenta)
                .Include(e => e.Pessoa)
                .Where(e => e.PessoaId == idUsuarioLogado)
                .AsQueryable();

            int itensPorPagina = 10;

            if (!string.IsNullOrEmpty(pesquisa))
            {
                consulta = consulta.Where(e =>
                    e.Ferramenta.Nome.Contains(pesquisa));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "D") consulta = consulta.Where(e => e.Status == "D");
                if (status == "U") consulta = consulta.Where(e => e.Status == "U");
                if (status == "A") consulta = consulta.Where(e => e.Status == "A");
            }

            if (dia.HasValue) consulta = consulta.Where(e => e.DataEmprestimo.Day == dia.Value);
            if (mes.HasValue) consulta = consulta.Where(e => e.DataEmprestimo.Month == mes.Value);
            if (ano.HasValue) consulta = consulta.Where(e => e.DataEmprestimo.Year == ano.Value);

            consulta = consulta.OrderByDescending(e => e.DataEmprestimo);

            int totalItens = await consulta.CountAsync();
            int totalPaginas = Math.Max(1, (int)Math.Ceiling((double)totalItens / itensPorPagina));

            var listaEmprestimos = await consulta
                .Skip((pagina - 1) * itensPorPagina)
                .Take(itensPorPagina)
                .ToListAsync();

            ViewBag.PaginaAtual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Pesquisa = pesquisa;
            ViewBag.Status = status;
            ViewBag.Dia = dia;
            ViewBag.Mes = mes;
            ViewBag.Ano = ano;

            return View(listaEmprestimos);
        }

        [HttpGet]
        public async Task<IActionResult> SolicitarFerramenta()
        {
            var ferramentasDisponiveis = await _context.Ferramentas
                .Where(f => f.Disponivel == true)
                .ToListAsync();

            ViewBag.Ferramentas = new SelectList(ferramentasDisponiveis, "Id", "Nome");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SolicitarFerramenta(int FerramentaId)
        {
            var ferramenta = await _context.Ferramentas.FindAsync(FerramentaId);

            if (ferramenta != null && ferramenta.Disponivel == true)
            {
                var novoEmprestimo = new EmprestimoModel
                {
                    FerramentaId = FerramentaId,
                    PessoaId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                    DataEmprestimo = DateTime.Now,
                    Status = "A"
                };

                ferramenta.Disponivel = false;

                _context.Emprestimos.Add(novoEmprestimo);
                _context.Ferramentas.Update(ferramenta);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewBag.Ferramentas = new SelectList(await _context.Ferramentas
                .Where(f => f.Disponivel == true)
                .ToListAsync(), "Id", "Nome", FerramentaId);

            return View(new EmprestimoModel());
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null) return NotFound();

            int idUsuarioLogado = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var emprestimo = await _context.Emprestimos
                .Include(e => e.Ferramenta)
                .Include(e => e.Pessoa)
                .FirstOrDefaultAsync(e => e.Id == id && e.PessoaId == idUsuarioLogado);

            if (emprestimo == null) return NotFound();

            return View(emprestimo);
        }
    }
}
