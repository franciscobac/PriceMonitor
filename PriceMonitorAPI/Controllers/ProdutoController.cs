using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceMonitorAPI.Dados;
using PriceMonitorAPI.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace PriceMonitorAPI.Controllers
{
    [Route("api/produtos")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly PriceMonitorContext _context;
        private readonly IConfiguration _configuration;
        List<Produto> produtosComAlteracaoPublica;

        public ProdutoController(PriceMonitorContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarProdutos([FromBody] List<Produto> produtos)
        {
            if (produtos == null || produtos.Count == 0)
                return BadRequest("Lista de produtos vazia.");

            try
            {
                // Converte os preços e ordena os produtos pelo menor preço
                var sortedProdutos = produtos
                    .Select(p => new Produto
                    {
                        Id = p.Id,
                        NomeProduto = p.NomeProduto,
                        PrecoProduto = p.PrecoProduto,
                        Url = p.Url,
                        Loja = p.Loja,
                        DataColeta = p.DataColeta,
                        PrecoDecimal = ConvertPrecoProduto(p.PrecoProduto) // Conversão do preço
                    })
                    .OrderBy(p => p.PrecoDecimal)
                    .ToList();

                // Obtém os produtos já salvos para comparação
                var produtosSalvos = await _context.Produtos.ToListAsync();

                // Lista para armazenar apenas produtos com alteração de preço
                var produtosComAlteracao = new List<Produto>();

                foreach (var novoProduto in sortedProdutos)
                {
                    var produtoExistente = produtosSalvos.FirstOrDefault(p => p.NomeProduto == novoProduto.NomeProduto && p.Loja == novoProduto.Loja);

                    if (produtoExistente == null || novoProduto.PrecoDecimal < produtoExistente.PrecoDecimal)
                    {
                        produtosComAlteracao.Add(novoProduto);
                    }
                }

                // Se nenhum produto teve alteração de preço, não grava no banco nem envia alerta
                if (!produtosComAlteracao.Any())
                {
                    return Ok(new { message = "Nenhum produto teve alteração de preço." });
                }

                // Adiciona apenas os produtos com alteração ao banco
                await _context.Produtos.AddRangeAsync(produtosComAlteracao);
                await _context.SaveChangesAsync();

                produtosComAlteracaoPublica = produtosComAlteracao;

                await EnviarAlertas(produtos);

                return Ok(new { message = $"{produtosComAlteracao.Count} produtos atualizados com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar no banco: {ex.Message}");
            }
        }

        private decimal ConvertPrecoProduto(string preco)
        {
            if (string.IsNullOrWhiteSpace(preco))
                return 0;

            return decimal.TryParse(preco.Replace("R$", "").Trim(), out var precoConvertido)
                ? precoConvertido
                : 0;
        }

        private async Task EnviarAlertas(IEnumerable<Produto> produtos)
        {
            var whatsappNumbers = _configuration.GetSection("NotificationSettings:WhatsAppNumbers").Get<List<string>>();

            if (whatsappNumbers == null || !whatsappNumbers.Any())
                return;

            // Agrupa os produtos em uma única mensagem
            string mensagem = "⚡ Atualização de preços:\n\n";

            // Seleciona apenas os quatro primeiros produtos
            var topProdutos = produtosComAlteracaoPublica.Take(4);

            foreach (var produto in topProdutos)
            {
                mensagem += $"📌 {produto.NomeProduto} - {produto.Loja}\n💰 Preço: {produto.PrecoProduto}\n🔗 {produto.Url}\n\n";
            }

            foreach (var numero in whatsappNumbers)
            {
                try
                {
                    await EnviarWhatsApp(numero, mensagem);
                    await Task.Delay(5000); // Espera 5 segundos antes do próximo envio
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao enviar mensagem via WhatsApp: {ex.Message}");
                }
            }

            try
            {
                await EnviarTelegram(mensagem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem via Telegram: {ex.Message}");
            }
        }

        private async Task EnviarTelegram(string mensagem)
        {
            var botToken = _configuration["Telegram:BotToken"];
            var chatId = _configuration["Telegram:ChatId"];

            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId))
            {
                Console.WriteLine("Token do Telegram ou Chat ID não configurado.");
                return;
            }

            string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(mensagem)}";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao enviar mensagem via Telegram: {errorMessage}");
                }
            }
        }

        private async Task EnviarWhatsApp(string numero, string mensagem)
        {
            var whatsappConfig = _configuration.GetSection("NotificationSettings:WhatsApp").Get<WhatsAppConfig>();

            var url = $"https://graph.facebook.com/v18.0/{whatsappConfig.PhoneNumberId}/messages";
            var payload = new
            {
                messaging_product = "whatsapp",
                to = numero,
                type = "text",
                text = new { body = mensagem }
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", whatsappConfig.AccessToken);
                var response = await client.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erro ao enviar mensagem via WhatsApp: {errorMessage}");
                }
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
