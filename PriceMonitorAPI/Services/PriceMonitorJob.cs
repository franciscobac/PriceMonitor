using System.Threading.Tasks;
using PriceMonitorAPI.Services;

namespace PriceMonitorAPI.Services
{
    public class PriceMonitorJob
    {
        private readonly NotificationService _notificationService;

        public PriceMonitorJob(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task MonitorPrices()
        {
            // Simulação de busca de preço mais baixo
            var priceFound = 99.99m;
            var message = $"Preço encontrado: R$ {priceFound}. Confira agora!";

            await _notificationService.SendEmailAlert("Novo Preço Encontrado!", message);
            await _notificationService.SendWhatsAppAlert(message);
        }
    }
}
