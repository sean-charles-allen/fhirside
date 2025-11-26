using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Api.Formatters;

/// <summary>
/// Input formatter that deserializes FHIR JSON to FHIR resources.
/// </summary>
public class FhirJsonInputFormatter : TextInputFormatter
{
    private static readonly JsonSerializerOptions FhirJsonOptions;

    static FhirJsonInputFormatter()
    {
        FhirJsonOptions = new JsonSerializerOptions().ForFhir();
    }

    public FhirJsonInputFormatter()
    {
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);

        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/fhir+json"));
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
    }

    protected override bool CanReadType(Type type)
    {
        return typeof(Base).IsAssignableFrom(type);
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var request = context.HttpContext.Request;
        using var reader = new StreamReader(request.Body, encoding);
        var content = await reader.ReadToEndAsync();

        try
        {
            var resource = JsonSerializer.Deserialize(content, context.ModelType, FhirJsonOptions);
            
            if (resource != null)
            {
                return await InputFormatterResult.SuccessAsync(resource);
            }
            
            return await InputFormatterResult.FailureAsync();
        }
        catch (JsonException)
        {
            return await InputFormatterResult.FailureAsync();
        }
    }
}
