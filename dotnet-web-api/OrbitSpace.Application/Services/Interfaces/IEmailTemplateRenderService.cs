using OrbitSpace.Application.Email;

namespace OrbitSpace.Application.Services.Interfaces
{
    public interface IEmailTemplateRenderService
    {
        string Render(IEmailTemplate template);
    }
}
