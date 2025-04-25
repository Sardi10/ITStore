using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace NewarkITStore.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // You can log this or leave it blank for now
            Console.WriteLine($"Simulated sending email to {email} with subject '{subject}'.");
            return Task.CompletedTask;
        }
    }
}
