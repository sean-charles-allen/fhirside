using FhirSide.Core.Models;

namespace FhirSide.Core.Services;

/// <summary>
/// Service interface for FHIR Bulk Data Access operations.
/// </summary>
public interface IBulkExportService
{
    /// <summary>
    /// Initiates a system-level bulk export job.
    /// Exports all resources from the system.
    /// </summary>
    /// <param name="baseUrl">Base URL for constructing download URLs.</param>
    /// <returns>The created export job.</returns>
    Task<BulkExportJob> StartSystemExportAsync(string baseUrl);

    /// <summary>
    /// Initiates a patient-level bulk export job.
    /// Exports all patient-related resources.
    /// </summary>
    /// <param name="baseUrl">Base URL for constructing download URLs.</param>
    /// <returns>The created export job.</returns>
    Task<BulkExportJob> StartPatientExportAsync(string baseUrl);

    /// <summary>
    /// Gets the status of an export job.
    /// </summary>
    /// <param name="jobId">The export job ID.</param>
    /// <returns>The export job, or null if not found.</returns>
    Task<BulkExportJob?> GetJobStatusAsync(string jobId);

    /// <summary>
    /// Gets the NDJSON content for an export file.
    /// </summary>
    /// <param name="jobId">The export job ID.</param>
    /// <param name="fileName">The file name.</param>
    /// <returns>The NDJSON content, or null if not found.</returns>
    Task<string?> GetExportFileAsync(string jobId, string fileName);

    /// <summary>
    /// Deletes an export job and its associated files.
    /// </summary>
    /// <param name="jobId">The export job ID.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteJobAsync(string jobId);
}
