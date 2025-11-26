using FhirSide.Core.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace FhirSide.Api.Controllers;

/// <summary>
/// Controller for FHIR MedicationRequest resources.
/// </summary>
[ApiController]
[Route("fhir/[controller]")]
[Produces("application/fhir+json")]
public class MedicationRequestController : ControllerBase
{
    private readonly IMedicationRequestService _medicationRequestService;
    private readonly ILogger<MedicationRequestController> _logger;

    public MedicationRequestController(IMedicationRequestService medicationRequestService, ILogger<MedicationRequestController> logger)
    {
        _medicationRequestService = medicationRequestService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all medication requests.
    /// </summary>
    /// <returns>A Bundle containing all medication requests.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetAll()
    {
        _logger.LogInformation("Getting all medication requests");
        var medicationRequests = await _medicationRequestService.GetAllAsync();
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = medicationRequests.Count(),
            Entry = medicationRequests.Select(m => new Bundle.EntryComponent
            {
                Resource = m,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/MedicationRequest/{m.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Gets a medication request by ID.
    /// </summary>
    /// <param name="id">The medication request ID.</param>
    /// <returns>The medication request resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MedicationRequest>> GetById(string id)
    {
        _logger.LogInformation("Getting medication request with ID: {Id}", id);
        var medicationRequest = await _medicationRequestService.GetByIdAsync(id);
        
        if (medicationRequest == null)
        {
            return NotFound();
        }
        
        return Ok(medicationRequest);
    }

    /// <summary>
    /// Gets medication requests for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <returns>A Bundle containing all medication requests for the patient.</returns>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetByPatient(string patientId)
    {
        _logger.LogInformation("Getting medication requests for patient: {PatientId}", patientId);
        var medicationRequests = await _medicationRequestService.GetByPatientIdAsync(patientId);
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = medicationRequests.Count(),
            Entry = medicationRequests.Select(m => new Bundle.EntryComponent
            {
                Resource = m,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/MedicationRequest/{m.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Creates a new medication request.
    /// </summary>
    /// <param name="medicationRequest">The medication request to create.</param>
    /// <returns>The created medication request.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicationRequest>> Create([FromBody] MedicationRequest medicationRequest)
    {
        _logger.LogInformation("Creating new medication request");
        var created = await _medicationRequestService.CreateAsync(medicationRequest);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing medication request.
    /// </summary>
    /// <param name="id">The medication request ID.</param>
    /// <param name="medicationRequest">The updated medication request.</param>
    /// <returns>The updated medication request.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MedicationRequest>> Update(string id, [FromBody] MedicationRequest medicationRequest)
    {
        _logger.LogInformation("Updating medication request with ID: {Id}", id);
        var updated = await _medicationRequestService.UpdateAsync(id, medicationRequest);
        
        if (updated == null)
        {
            return NotFound();
        }
        
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a medication request.
    /// </summary>
    /// <param name="id">The medication request ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting medication request with ID: {Id}", id);
        var deleted = await _medicationRequestService.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}
