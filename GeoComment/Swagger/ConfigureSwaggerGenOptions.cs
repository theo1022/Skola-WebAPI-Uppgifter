using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GeoComment.Services
{
    public class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions o)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                o.SwaggerDoc(description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = "Versioning by url",
                        Version = description.ApiVersion.ToString()
                    });
            }
        }
    }
}
