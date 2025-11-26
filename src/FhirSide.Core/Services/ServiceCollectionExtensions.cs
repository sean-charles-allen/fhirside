using Microsoft.Extensions.DependencyInjection;

namespace FhirSide.Core.Services;

/// <summary>
/// Extension methods for registering FHIR services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds FHIR services with in-memory storage to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFhirServices(this IServiceCollection services)
    {
        services.AddSingleton<IPatientService, InMemoryPatientService>();
        services.AddSingleton<IEncounterService, InMemoryEncounterService>();
        services.AddSingleton<IObservationService, InMemoryObservationService>();
        services.AddSingleton<IMedicationRequestService, InMemoryMedicationRequestService>();
        
        return services;
    }
}
