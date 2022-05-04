using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GeoComment.Swagger
{
    public class AddApiVersionExampleValueOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameterName = "api-version";

            var parameter = operation
                .Parameters
                .First(p => p.Name == versionParameterName);
            var description = context
                .ApiDescription
                .ParameterDescriptions
                .First(p => p.Name == versionParameterName);

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null)
            {
                parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
            }
        }
    }
}
