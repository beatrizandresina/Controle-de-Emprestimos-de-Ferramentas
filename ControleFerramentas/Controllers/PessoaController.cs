using ControleFerramentas.Data;
using ControleFerramentas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleFerramentas.Controllers
{
    public class PessoaController : Controller
    {
            private readonly ILogger<PessoaController> _logger;
            private readonly AppDbContext _context;
            public PessoaController(ILogger<PessoaController> logger, AppDbContext context)
            {
                _logger = logger;
                _context = context;
            }
            [HttpGet]
            public async Task<IActionResult> Index()
            {
                var listaPessoas = await _context.Pessoas.ToListAsync();
                return View(listaPessoas);
            }

            [HttpGet]
            public async Task<IActionResult> DetalhesPessoa(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }
            var pessoa = await _context.Pessoas.FirstOrDefaultAsync(f => f.Id == id);
                if (pessoa == null)
                {
                    return NotFound();
                }
                return View(pessoa);
            }

            [HttpGet]
            public IActionResult CadastrarPessoa()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> CadastrarPessoa(PessoaModel pessoa)
            {
                if (ModelState.IsValid)
                {
                    _context.Pessoas.Add(pessoa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(pessoa);
            }

            [HttpGet]
            public async Task<IActionResult> EditarPessoa(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var pessoa = await _context.Pessoas.FindAsync(id);
                if (pessoa == null)
                {
                    return NotFound();
                }
                return View(pessoa);
            }

            [HttpPost]
            public async Task<IActionResult> EditarPessoa(int id, PessoaModel pessoaModel)
            {
                if (ModelState.IsValid)
                {
                    var pessoa = await _context.Pessoas.FindAsync(id);
                    if (pessoa == null)
                    {
                        return NotFound();
                    }
                    pessoa.Nome = pessoaModel.Nome;
                    pessoa.Email = pessoaModel.Email;
                    pessoa.Telefone = pessoaModel.Telefone;
                    pessoa.Senha = pessoaModel.Senha;
                    pessoa.Perfil = pessoaModel.Perfil;
                    _context.Pessoas.Update(pessoa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(pessoaModel);
            }

            [HttpGet]
            public async Task<IActionResult> ExcluirPessoa(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var pessoa = await _context.Pessoas.FirstOrDefaultAsync(f => f.Id == id);
                if (pessoa == null)
                {
                    return NotFound();
                }
                return View(pessoa);
            }

            [HttpPost, ActionName("ExcluirPessoa")]
            public async Task<IActionResult> ExcluirPessoa(int id)
            {
                var pessoa = await _context.Pessoas.FirstOrDefaultAsync(f => f.Id == id);
                if (pessoa == null)
                {
                    return NotFound();
                }
                _context.Pessoas.Remove(pessoa);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
        }
    }