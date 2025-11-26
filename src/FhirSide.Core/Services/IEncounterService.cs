using Hl7.Fhir.Model;

namespace FhirSide.Core.Services;

/// <summary>
/// Service interface for FHIR Encounter resources.
/// </summary>
public interface IEncounterService
{
    /// <summary>
    /// Gets all encounters.
    /// </summary>
    /// <returns>A collection of encounters.</returns>
    Task<IEnumerable<Encounter>> GetAllAsync();
    
    /// <summary>
    /// Gets an encounter by ID.
    /// </summary>
    /// <param name="id">The encounter ID.</param>
    /// <returns>The encounter, or null if not found.</returns>
    Task<Encounter?> GetByIdAsync(string id);
    
    /// <summary>
    /// Gets encounters for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <returns>A collection of encounters for the patient.</returns>
    Task<IEnumerable<Encounter>> GetByPatientIdAsync(string patientId);
    
    /// <summary>
    /// Creates a new encounter.
    /// </summary>
    /// <param name="encounter">The encounter to create.</param>
    /// <returns>The created encounter.</returns>
    Task<Encounter> CreateAsync(Encounter encounter);
    
    /// <summary>
    /// Updates an existing encounter.
    /// </summary>
    /// <param name="id">The encounter ID.</param>
    /// <param name="encounter">The updated encounter.</param>
    /// <returns>The updated encounter, or null if not found.</returns>
    Task<Encounter?> UpdateAsync(string id, Encounter encounter);
    
    /// <summary>
    /// Deletes an encounter.
    /// </summary>
    /// <param name="id">The encounter ID.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(string id);
}
