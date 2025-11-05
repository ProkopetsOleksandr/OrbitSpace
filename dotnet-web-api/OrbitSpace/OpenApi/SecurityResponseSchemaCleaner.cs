using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace OrbitSpace.WebApi.OpenApi;

public class SecurityResponseSchemaCleaner : IOpenApiOperationTransformer
{
    public ValueTask TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        // 1. Проверяем, есть ли на эндпоинте атрибут [Authorize]
        var isAuthorizedEndpoint = context.Description.EndpointMetadata
            .OfType<AuthorizeAttribute>()
            .Any();

        if (isAuthorizedEndpoint)
        {
            var codesToClean = new[] 
            { 
                StatusCodes.Status401Unauthorized.ToString(), 
                StatusCodes.Status403Forbidden.ToString() 
            };

            foreach (var code in codesToClean)
            {
                if (operation.Responses.TryGetValue(code, out var response))
                {
                    // 2. Принудительно очищаем контент ответа.
                    // Это удалит неверно выведенный ProblemDetails и оставит ответ без тела (Content: {}).
                    response.Content.Clear();
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}