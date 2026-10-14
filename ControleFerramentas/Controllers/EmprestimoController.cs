using ControleFerramentas.Data;
using ControleFerramentas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControleFerramentas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmprestimoController : Controller
    {
        private readonly ILogger<EmprestimoController> _logger;
        private readonly AppDbContext _context;
        public EmprestimoController(ILogger<EmprestimoController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string pesquisa, string status, int? dia, int? mes, int? ano, int pagina = 1)
        {
            int itensPorPagina = 10;

            var consulta = _context.Emprestimos
                .Include(e => e.Ferramenta)
                .Include(e => e.Pessoa)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisa))
            {
                consulta = consulta.Where(e =>
                    e.Pessoa.Nome.Contains(pesquisa) ||
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
        public async Task<IActionResult> CadastrarEmprestimo()
        {
            var pessoas = await _context.Pessoas.ToListAsync();
            var ferramentasDisponiveis = await _context.Ferramentas
                .Where(f => f.Disponivel == true)
                .ToListAsync();

            ViewBag.Pessoas = new SelectList(pessoas, "Id", "Nome");
            ViewBag.Ferramentas = new SelectList(ferramentasDisponiveis, "Id", "Nome");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarEmprestimo(EmprestimoModel emprestimoModel)
        {
            ModelState.Remove("Ferramenta");
            ModelState.Remove("Pessoa");

            ModelState.Remove("Status");
            ModelState.Remove("DataEmprestimo");
            if (ModelState.IsValid)
            {
                var ferramenta = await _context.Ferramentas.FindAsync(emprestimoModel.FerramentaId);

                emprestimoModel.Status = "U";
                if (ferramenta != null && ferramenta.Disponivel == true)
                {
                    emprestimoModel.DataEmprestimo = DateTime.Now;
                    ferramenta.Disponivel = false;

                    _context.Emprestimos.Add(emprestimoModel);
                    _context.Ferramentas.Update(ferramenta);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Pessoas = new SelectList(await _context.Pessoas.ToListAsync(), "Id", "Nome", emprestimoModel.PessoaId);
            ViewBag.Ferramentas = new SelectList(await _context.Ferramentas.Where(f => f.Disponivel == true).ToListAsync(), "Id", "Nome", emprestimoModel.FerramentaId);

            return View(emprestimoModel);
        }

        [HttpGet]
        public async Task<IActionResult> DetalhesEmprestimo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Ferramenta)
                .Include(e => e.Pessoa)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }
            return View(emprestimo);
        }

        [HttpGet]
        public async Task<IActionResult> DevolverEmprestimo(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Ferramenta)
                .Include(e => e.Pessoa)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }
            return View(emprestimo);
        }

        [HttpPost, ActionName("DevolverEmprestimo")]
        public async Task<IActionResult> ConfirmarDevolucao(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            var ferramenta = await _context.Ferramentas.FindAsync(emprestimo.FerramentaId);

            emprestimo.Status = "D";
            emprestimo.DataDevolucao = DateTime.Now;
            if (ferramenta != null)
            {
                ferramenta.Disponivel = true;
                _context.Ferramentas.Update(ferramenta);
            }
            _context.Emprestimos.Update(emprestimo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EntregarFerramenta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var emprestimo = await _context.Emprestimos
                .Include(e => e.Ferramenta)
                .Include(e => e.Pessoa)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }
            return View(emprestimo);
        }

        [HttpPost, ActionName("EntregarFerramenta")]
        public async Task<IActionResult> ConfirmarEntrega(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            var ferramenta = await _context.Ferramentas.FindAsync(emprestimo.FerramentaId);
            emprestimo.DataEmprestimo = DateTime.Now;
            emprestimo.Status = "U";
            if (ferramenta != null)
            {
                ferramenta.Disponivel = false;
                _context.Ferramentas.Update(ferramenta);
            }
            _context.Emprestimos.Update(emprestimo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
