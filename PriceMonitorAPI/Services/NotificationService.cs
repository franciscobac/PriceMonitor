using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceMonitorAPI.Services
{
    public class NotificationService
    {
        private readonly IConfiguration _configuration;

        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAlert(string subject, string message)
        {
            var emailList = _configuration.GetSection("AlertConfig:Emails").Get<List<string>>();

            using (var smtpClient = new SmtpClient("smtp.seuprovedor.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("seuemail@example.com", "suasenha");
                smtpClient.EnableSsl = true;

                foreach (var email in emailList)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("seuemail@example.com"),
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(email);
                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
        }

        public async Task SendWhatsAppAlert(string message)
        {
            var numbers = _configuration.GetSection("AlertConfig:WhatsAppNumbers").Get<List<string>>();

            foreach (var number in numbers)
            {
                // Simulação de envio de WhatsApp
                await Task.Delay(500);
                System.Console.WriteLine($"Mensagem enviada para {number}: {message}");
            }
        }
    }
}
