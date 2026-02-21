using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using OrbitSpace.Application.Email;
using OrbitSpace.Application.Services.Interfaces;

namespace OrbitSpace.Infrastructure.Services
{
    public sealed partial class EmailTemplateRenderService(ILogger<EmailTemplateRenderService> logger) : IEmailTemplateRenderService
    {
        private static readonly ConcurrentDictionary<string, string> TemplateCache = new();
        private readonly Assembly _templateAssembly = Assembly.GetExecutingAssembly();

        public string Render(IEmailTemplate template)
        {
            var html = GetTemplateHtml(template.TemplateName);

            return ReplacePlaceholders(html, template.GetParameters());
        }

        private string GetTemplateHtml(string templateName)
        {
            if (string.IsNullOrWhiteSpace(templateName))
                throw new ArgumentException("Template name cannot be empty.", nameof(templateName));

            return TemplateCache.GetOrAdd(templateName, name =>
            {
                var resourceName = $"{_templateAssembly.GetName().Name}.Email.Templates.{name}.html";

                using var stream = _templateAssembly.GetManifestResourceStream(resourceName)
                    ?? throw new InvalidOperationException(
                        $"Embedded resource '{resourceName}' not found. " +
                        $"Available resources: {string.Join(", ", _templateAssembly.GetManifestResourceNames())}");

                using var reader = new StreamReader(stream, Encoding.UTF8);

                logger.LogDebug("Loaded and cached email template '{TemplateName}'", name);

                return reader.ReadToEnd();
            });
        }

        private static string ReplacePlaceholders(string html, IReadOnlyDictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
            {
                return html;
            }

            return PlaceholderRegex().Replace(html, match =>
            {
                var key = match.Groups[1].Value;
                if (parameters.TryGetValue(key, out var value))
                {
                    return value;
                }

                return match.Value;
            });
        }

        [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
        private static partial Regex PlaceholderRegex();
    }
}
