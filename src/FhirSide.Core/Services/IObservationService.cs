using Hl7.Fhir.Model;

namespace FhirSide.Core.Services;

/// <summary>
/// Service interface for FHIR Observation resources.
/// </summary>
public interface IObservationService
{
    /// <summary>
    /// Gets all observations.
    /// </summary>
    /// <returns>A collection of observations.</returns>
    Task<IEnumerable<Observation>> GetAllAsync();
    
    /// <summary>
    /// Gets an observation by ID.
    /// </summary>
    /// <param name="id">The observation ID.</param>
    /// <returns>The observation, or null if not found.</returns>
    Task<Observation?> GetByIdAsync(string id);
    
    /// <summary>
    /// Gets observations for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <returns>A collection of observations for the patient.</returns>
    Task<IEnumerable<Observation>> GetByPatientIdAsync(string patientId);
    
    /// <summary>
    /// Creates a new observation.
    /// </summary>
    /// <param name="observation">The observation to create.</param>
    /// <returns>The created observation.</returns>
    Task<Observation> CreateAsync(Observation observation);
    
    /// <summary>
    /// Updates an existing observation.
    /// </summary>
    /// <param name="id">The observation ID.</param>
    /// <param name="observation">The updated observation.</param>
    /// <returns>The updated observation, or null if not found.</returns>
    Task<Observation?> UpdateAsync(string id, Observation observation);
    
    /// <summary>
    /// Deletes an observation.
    /// </summary>
    /// <param name="id">The observation ID.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(string id);
}
