using FhirSide.Core.Services;
using Hl7.Fhir.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure JSON serialization for FHIR resources
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add FHIR services
builder.Services.AddFhirServices();

// Add health checks
builder.Services.AddHealthChecks();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FhirSide API",
        Version = "v1",
        Description = "A FHIR R4 compliant sandbox API for healthcare data interoperability",
        Contact = new OpenApiContact
        {
            Name = "FhirSide Team",
            Url = new Uri("https://github.com/sean-charles-allen/fhirside")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // Fix schema ID conflicts for FHIR nested and generic types
    options.CustomSchemaIds(type =>
    {
        var typeName = type.Name;

        // Handle generic types by including type arguments
        if (type.IsGenericType)
        {
            var genericTypeName = type.GetGenericTypeDefinition().Name;
            var genericArgs = type.GetGenericArguments();
            var genericArgNames = string.Join("", genericArgs.Select(arg =>
            {
                if (arg.DeclaringType != null)
                {
                    return $"{arg.DeclaringType.Name}{arg.Name}";
                }
                return arg.Name;
            }));

            typeName = $"{genericTypeName.Split('`')[0]}{genericArgNames}";
        }

        // Handle nested types
        if (type.DeclaringType != null)
        {
            return $"{type.DeclaringType.Name}{typeName}";
        }

        return typeName;
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FhirSide API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at the root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map health check endpoint
app.MapHealthChecks("/health");

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }
