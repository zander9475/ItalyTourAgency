using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ItalyTourAgency
{
    // Add this file to your project
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Email to {email} with subject '{subject}' would be sent here");
            return Task.CompletedTask;
        }
    }
}