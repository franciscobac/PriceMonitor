namespace PriceMonitorAPI.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string NomeProduto { get; set; }
        public string PrecoProduto { get; set; }
        public decimal PrecoDecimal { get; set; }
        public string Url { get; set; }
        public string Loja { get; set; }
        public DateTime DataColeta { get; set; }
    }
}
