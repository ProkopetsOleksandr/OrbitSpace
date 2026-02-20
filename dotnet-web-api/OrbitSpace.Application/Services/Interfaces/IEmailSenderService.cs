using OrbitSpace.Application.Common.Models;

namespace OrbitSpace.Application.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendAsync(EmailMessage message);
}