using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Api.Formatters;

/// <summary>
/// Output formatter that serializes FHIR resources using the FHIR JSON serializer.
/// </summary>
/// <remarks>
/// This formatter uses the Hl7.Fhir SDK's ToJson extension method to produce
/// FHIR-compliant JSON output. The Task alias is required due to ambiguity
/// between System.Threading.Tasks.Task and Hl7.Fhir.Model.Task (FHIR Task resource).
/// </remarks>
public class FhirJsonOutputFormatter : TextOutputFormatter
{
    public FhirJsonOutputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
    }

    protected override bool CanWriteType(Type? type)
    {
        if (type == null)
            return false;

        return typeof(Base).IsAssignableFrom(type);
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
    {
        var response = context.HttpContext.Response;

        // CanWriteType ensures we only get called for Base types, but we check defensively
        if (context.Object is Base fhirResource)
        {
            // Use the FHIR SDK's ToJson extension method for proper FHIR-compliant JSON serialization
            var json = fhirResource.ToJson();
            await response.WriteAsync(json, selectedEncoding);
        }
    }
}
