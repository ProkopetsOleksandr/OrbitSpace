using OrbitSpace.Application.Common.Models;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.Infrastructure.Services;

public class EmailSenderService : IEmailSenderService
{
    public Task SendAsync(EmailMessage message)
    {
        Console.WriteLine($"Email with subject  {message.Subject} sent to {message.To}");
        return Task.CompletedTask;
    }
}