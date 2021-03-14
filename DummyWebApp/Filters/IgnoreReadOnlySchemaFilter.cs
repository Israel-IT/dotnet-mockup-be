namespace DummyWebApp.Filters
{
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "ASP NET will instantiate")]
    public class IgnoreReadOnlySchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.ReadOnly = false;

            if (schema.Properties != null)
            {
                foreach (var keyValuePair in schema.Properties)
                {
                    keyValuePair.Value.ReadOnly = false;
                }
            }
        }
    }
}