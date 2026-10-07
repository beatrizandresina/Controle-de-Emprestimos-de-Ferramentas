using ControleFerramentas.Data;
using ControleFerramentas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFerramentas.Controllers
{
    public class FerramentaController : Controller
    {
        private readonly ILogger<FerramentaController> _logger;
        private readonly AppDbContext _context;
        public FerramentaController(ILogger<FerramentaController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listaFerramentas = await _context.Ferramentas.ToListAsync();
            return View(listaFerramentas);
        }

        [HttpGet]
        public async Task<IActionResult> DetalhesFerramenta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ferramenta = await _context.Ferramentas.FirstOrDefaultAsync(f => f.Id == id);
            if (ferramenta == null)
            {
                return NotFound();
            }
            return View(ferramenta);
        }

        [HttpGet]
        public IActionResult CadastrarFerramenta()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarFerramenta(FerramentaModel ferramenta)
        {
            if (ModelState.IsValid)
            {
                ferramenta.DataCadastro = DateTime.Now;
                _context.Ferramentas.Add(ferramenta);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ferramenta);
        }

        [HttpGet]
        public async Task<IActionResult> EditarFerramenta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ferramenta = await _context.Ferramentas.FindAsync(id);
            if (ferramenta == null)
            {
                return NotFound();
            }
            return View(ferramenta);
        }

        [HttpPost]
        public async Task<IActionResult> EditarFerramenta(int id, FerramentaModel ferramentaModel)
        {
            if (ModelState.IsValid)
            {
                var ferramenta = await _context.Ferramentas.FindAsync(id);
                if (ferramenta == null)
                {
                    return NotFound();
                }
                ferramenta.Nome = ferramentaModel.Nome;
                ferramenta.Descricao = ferramentaModel.Descricao;
                ferramenta.Observacao = ferramentaModel.Observacao;
                ferramenta.Disponivel = ferramentaModel.Disponivel;
                _context.Ferramentas.Update(ferramenta);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ferramentaModel);
        }

        [HttpGet]
        public async Task<IActionResult> ExcluirFerramenta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var ferramenta = await _context.Ferramentas.FirstOrDefaultAsync(f => f.Id == id);
            if (ferramenta == null)
            {
                return NotFound();
            }
            return View(ferramenta);
        }

        [HttpPost, ActionName("ExcluirFerramenta")]
        public async Task<IActionResult> ExcluirFerramenta(int id)
        {
            var ferramenta = await _context.Ferramentas.FirstOrDefaultAsync(f => f.Id == id);
            if (ferramenta == null)
            {
                return NotFound();
            }
            _context.Ferramentas.Remove(ferramenta);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}