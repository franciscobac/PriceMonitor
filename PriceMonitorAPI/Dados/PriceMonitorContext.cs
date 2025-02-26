using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PriceMonitorAPI.Models;

namespace PriceMonitorAPI.Dados
{
    public class PriceMonitorContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        private readonly IConfiguration _configuration;

        public PriceMonitorContext(DbContextOptions<PriceMonitorContext> options, IConfiguration configuration) 
            : base(options) 
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
