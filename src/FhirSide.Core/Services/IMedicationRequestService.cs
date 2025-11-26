using Hl7.Fhir.Model;

namespace FhirSide.Core.Services;

/// <summary>
/// Service interface for FHIR MedicationRequest resources.
/// </summary>
public interface IMedicationRequestService
{
    /// <summary>
    /// Gets all medication requests.
    /// </summary>
    /// <returns>A collection of medication requests.</returns>
    Task<IEnumerable<MedicationRequest>> GetAllAsync();
    
    /// <summary>
    /// Gets a medication request by ID.
    /// </summary>
    /// <param name="id">The medication request ID.</param>
    /// <returns>The medication request, or null if not found.</returns>
    Task<MedicationRequest?> GetByIdAsync(string id);
    
    /// <summary>
    /// Gets medication requests for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <returns>A collection of medication requests for the patient.</returns>
    Task<IEnumerable<MedicationRequest>> GetByPatientIdAsync(string patientId);
    
    /// <summary>
    /// Creates a new medication request.
    /// </summary>
    /// <param name="medicationRequest">The medication request to create.</param>
    /// <returns>The created medication request.</returns>
    Task<MedicationRequest> CreateAsync(MedicationRequest medicationRequest);
    
    /// <summary>
    /// Updates an existing medication request.
    /// </summary>
    /// <param name="id">The medication request ID.</param>
    /// <param name="medicationRequest">The updated medication request.</param>
    /// <returns>The updated medication request, or null if not found.</returns>
    Task<MedicationRequest?> UpdateAsync(string id, MedicationRequest medicationRequest);
    
    /// <summary>
    /// Deletes a medication request.
    /// </summary>
    /// <param name="id">The medication request ID.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(string id);
}
