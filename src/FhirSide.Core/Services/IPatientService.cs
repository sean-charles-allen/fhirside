using Hl7.Fhir.Model;

namespace FhirSide.Core.Services;

/// <summary>
/// Service interface for FHIR Patient resources.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Gets all patients.
    /// </summary>
    /// <returns>A collection of patients.</returns>
    Task<IEnumerable<Patient>> GetAllAsync();
    
    /// <summary>
    /// Gets a patient by ID.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <returns>The patient, or null if not found.</returns>
    Task<Patient?> GetByIdAsync(string id);
    
    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="patient">The patient to create.</param>
    /// <returns>The created patient.</returns>
    Task<Patient> CreateAsync(Patient patient);
    
    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <param name="patient">The updated patient.</param>
    /// <returns>The updated patient, or null if not found.</returns>
    Task<Patient?> UpdateAsync(string id, Patient patient);
    
    /// <summary>
    /// Deletes a patient.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(string id);
}
