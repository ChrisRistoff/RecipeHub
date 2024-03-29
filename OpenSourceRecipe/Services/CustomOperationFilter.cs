using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var fileUploadParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile))
            .Select(p => p.Name)
            .ToList();

        if (fileUploadParams.Any())
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileUploadParams.ToDictionary(
                                name =>
                                {
                                    if (name == null) throw new ArgumentNullException(nameof(name));
                                    return name;
                                },
                                name =>
                                {
                                    if (name == null) throw new ArgumentNullException(nameof(name));
                                    return new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    };
                                })
                        }
                    }
                }
            };
        }
    }
}
