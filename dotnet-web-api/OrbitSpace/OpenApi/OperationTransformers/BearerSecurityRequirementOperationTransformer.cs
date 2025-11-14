using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace OrbitSpace.WebApi.OpenApi.OperationTransformers
{
    public class BearerSecurityRequirementOperationTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var metadata = context.Description.ActionDescriptor.EndpointMetadata;

            bool hasAuthorize = metadata.Any(m => m is AuthorizeAttribute);
            bool hasAllowAnonymous = metadata.Any(m => m is IAllowAnonymous);

            if (hasAuthorize && !hasAllowAnonymous)
            {
                var requirement = new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", context.Document)] = new List<string>()
                };

                operation.Security ??= new List<OpenApiSecurityRequirement>();
                if (!operation.Security.Any(r => r.ContainsKey(requirement.Keys.First())))
                {
                    operation.Security.Add(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
