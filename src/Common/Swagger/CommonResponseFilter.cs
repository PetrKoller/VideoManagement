namespace Common.Swagger;

/// <summary>
/// Adds common responses to swagger documentation.
/// </summary>
public sealed class CommonResponseFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        bool allowAnonymous = context.ApiDescription.ActionDescriptor.EndpointMetadata
            .Any(x => x is AllowAnonymousAttribute);

        if (!allowAnonymous)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
        }

        var schema = context.SchemaGenerator.GenerateSchema(typeof(Failure), context.SchemaRepository);
        operation.Responses.Add("500", new OpenApiResponse
        {
            Description = "Internal Server Error",
            Content = new Dictionary<string, OpenApiMediaType>
            {
                ["application/json"] = new()
                {
                    Schema = schema,
                },
            },
        });
    }
}
