namespace OrbitSpace.Application.Email
{
    public interface IEmailTemplate
    {
        string TemplateName { get; }
        IReadOnlyDictionary<string, string> GetParameters();
    }
}
