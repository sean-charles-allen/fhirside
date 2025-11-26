using FhirSide.Core.Models;
using FhirSide.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace FhirSide.Api.Controllers;

/// <summary>
/// Controller for FHIR Bulk Data Access operations.
/// Implements the FHIR Bulk Data Access IG (https://hl7.org/fhir/uv/bulkdata/).
/// </summary>
[ApiController]
[Route("fhir")]
public class BulkExportController : ControllerBase
{
    private readonly IBulkExportService _bulkExportService;
    private readonly ILogger<BulkExportController> _logger;

    public BulkExportController(IBulkExportService bulkExportService, ILogger<BulkExportController> logger)
    {
        _bulkExportService = bulkExportService;
        _logger = logger;
    }

    /// <summary>
    /// Initiates a system-level bulk export.
    /// Exports all resources from the system asynchronously.
    /// </summary>
    /// <remarks>
    /// This endpoint initiates an asynchronous export of all FHIR resources.
    /// Returns 202 Accepted with a Content-Location header pointing to the status endpoint.
    /// Poll the status endpoint to check progress and get download URLs when complete.
    /// </remarks>
    /// <returns>202 Accepted with Content-Location header for status polling.</returns>
    [HttpGet("$export")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> StartSystemExport()
    {
        _logger.LogInformation("Starting system-level bulk export");

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var job = await _bulkExportService.StartSystemExportAsync(baseUrl);

        var statusUrl = $"{baseUrl}/fhir/$export-status/{job.Id}";

        Response.Headers.Append("Content-Location", statusUrl);
        return Accepted();
    }

    /// <summary>
    /// Initiates a patient-level bulk export.
    /// Exports all patient-related resources asynchronously.
    /// </summary>
    /// <remarks>
    /// This endpoint initiates an asynchronous export of all patient-related FHIR resources.
    /// Returns 202 Accepted with a Content-Location header pointing to the status endpoint.
    /// Poll the status endpoint to check progress and get download URLs when complete.
    /// </remarks>
    /// <returns>202 Accepted with Content-Location header for status polling.</returns>
    [HttpGet("Patient/$export")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> StartPatientExport()
    {
        _logger.LogInformation("Starting patient-level bulk export");

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var job = await _bulkExportService.StartPatientExportAsync(baseUrl);

        var statusUrl = $"{baseUrl}/fhir/$export-status/{job.Id}";

        Response.Headers.Append("Content-Location", statusUrl);
        return Accepted();
    }

    /// <summary>
    /// Gets the status of a bulk export job.
    /// </summary>
    /// <remarks>
    /// Returns the current status of an export job.
    /// - If in progress: Returns 202 Accepted with X-Progress header.
    /// - If completed: Returns 200 OK with download URLs in the response body.
    /// - If failed: Returns 500 Internal Server Error with error details.
    /// </remarks>
    /// <param name="jobId">The export job ID.</param>
    /// <returns>Status of the export job.</returns>
    [HttpGet("$export-status/{jobId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetExportStatus(string jobId)
    {
        _logger.LogInformation("Getting export status for job: {JobId}", jobId);

        var job = await _bulkExportService.GetJobStatusAsync(jobId);

        if (job == null)
        {
            return NotFound(new { error = "Export job not found" });
        }

        return job.Status switch
        {
            BulkExportStatus.InProgress => GetInProgressResponse(job),
            BulkExportStatus.Completed => GetCompletedResponse(job),
            BulkExportStatus.Failed => GetFailedResponse(job),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unknown job status" })
        };
    }

    /// <summary>
    /// Downloads an exported NDJSON file.
    /// </summary>
    /// <remarks>
    /// Downloads the NDJSON file for a specific resource type from a completed export.
    /// The file contains one JSON object per line (newline-delimited JSON).
    /// </remarks>
    /// <param name="jobId">The export job ID.</param>
    /// <param name="fileName">The file name (e.g., Patient.ndjson).</param>
    /// <returns>The NDJSON file content.</returns>
    [HttpGet("$export-download/{jobId}/{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadExportFile(string jobId, string fileName)
    {
        _logger.LogInformation("Downloading export file {FileName} for job: {JobId}", fileName, jobId);

        var content = await _bulkExportService.GetExportFileAsync(jobId, fileName);

        if (content == null)
        {
            return NotFound(new { error = "Export file not found" });
        }

        return Content(content, "application/ndjson");
    }

    /// <summary>
    /// Deletes a bulk export job and its associated files.
    /// </summary>
    /// <param name="jobId">The export job ID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("$export-status/{jobId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExportJob(string jobId)
    {
        _logger.LogInformation("Deleting export job: {JobId}", jobId);

        var deleted = await _bulkExportService.DeleteJobAsync(jobId);

        if (!deleted)
        {
            return NotFound(new { error = "Export job not found" });
        }

        return NoContent();
    }

    private IActionResult GetInProgressResponse(BulkExportJob job)
    {
        Response.Headers.Append("X-Progress", "Export in progress");
        return Accepted();
    }

    private IActionResult GetCompletedResponse(BulkExportJob job)
    {
        var response = new
        {
            transactionTime = job.CompletedAt?.ToString("o"),
            request = $"{job.BaseUrl}/fhir/$export",
            requiresAccessToken = false,
            output = job.Output.Select(o => new
            {
                type = o.Type,
                url = o.Url,
                count = o.Count
            }).ToList(),
            error = Array.Empty<object>()
        };

        return Ok(response);
    }

    private IActionResult GetFailedResponse(BulkExportJob job)
    {
        return StatusCode(StatusCodes.Status500InternalServerError, new
        {
            error = job.ErrorMessage ?? "Export failed"
        });
    }
}
