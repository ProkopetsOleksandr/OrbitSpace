namespace OrbitSpace.Application.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendAsync(string subject, string to, string body, CancellationToken cancellationToken = default);
}