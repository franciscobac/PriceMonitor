using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceMonitorAPI.Dados;
using PriceMonitorAPI.Models;

namespace PriceMonitorAPI.Controllers
{
    [Route("api/produtos")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly PriceMonitorContext _context;

        public ProdutoController(PriceMonitorContext context)
        {
            _context = context;
        }

        //[HttpPost]
        // public IActionResult Post([FromBody] Produto produto)
        // {
        //     _context.Produtos.Add(produto);
        //     _context.SaveChanges();
        //     return Ok(new { message = "Produto salvo com sucesso!" });
        // }

        [HttpPost]
        public async Task<IActionResult> AdicionarProdutos([FromBody] List<Produto> produtos)
        {
            if (produtos == null || produtos.Count == 0)
                return BadRequest("Lista de produtos vazia.");

            try
            {
                _context.Produtos.AddRange(produtos); // Adiciona todos os produtos à tabela
                await _context.SaveChangesAsync(); // Salva as mudanças no banco

                return Ok(new { message = $"{produtos.Count} produtos adicionados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar no banco: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return NotFound();

            return Ok(produto);
        }
    }
}
