using FhirSide.Core.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace FhirSide.Api.Controllers;

/// <summary>
/// Controller for FHIR Encounter resources.
/// </summary>
[ApiController]
[Route("fhir/[controller]")]
[Produces("application/fhir+json")]
public class EncounterController : ControllerBase
{
    private readonly IEncounterService _encounterService;
    private readonly ILogger<EncounterController> _logger;

    public EncounterController(IEncounterService encounterService, ILogger<EncounterController> logger)
    {
        _encounterService = encounterService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all encounters.
    /// </summary>
    /// <returns>A Bundle containing all encounters.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetAll()
    {
        _logger.LogInformation("Getting all encounters");
        var encounters = await _encounterService.GetAllAsync();
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = encounters.Count(),
            Entry = encounters.Select(e => new Bundle.EntryComponent
            {
                Resource = e,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/Encounter/{e.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Gets an encounter by ID.
    /// </summary>
    /// <param name="id">The encounter ID.</param>
    /// <returns>The encounter resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Encounter>> GetById(string id)
    {
        _logger.LogInformation("Getting encounter with ID: {Id}", id);
        var encounter = await _encounterService.GetByIdAsync(id);
        
        if (encounter == null)
        {
            return NotFound();
        }
        
        return Ok(encounter);
    }

    /// <summary>
    /// Gets encounters for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <returns>A Bundle containing all encounters for the patient.</returns>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetByPatient(string patientId)
    {
        _logger.LogInformation("Getting encounters for patient: {PatientId}", patientId);
        var encounters = await _encounterService.GetByPatientIdAsync(patientId);
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = encounters.Count(),
            Entry = encounters.Select(e => new Bundle.EntryComponent
            {
                Resource = e,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/Encounter/{e.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Creates a new encounter.
    /// </summary>
    /// <param name="encounter">The encounter to create.</param>
    /// <returns>The created encounter.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Encounter>> Create([FromBody] Encounter encounter)
    {
        _logger.LogInformation("Creating new encounter");
        var created = await _encounterService.CreateAsync(encounter);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing encounter.
    /// </summary>
    /// <param name="id">The encounter ID.</param>
    /// <param name="encounter">The updated encounter.</param>
    /// <returns>The updated encounter.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Encounter>> Update(string id, [FromBody] Encounter encounter)
    {
        _logger.LogInformation("Updating encounter with ID: {Id}", id);
        var updated = await _encounterService.UpdateAsync(id, encounter);
        
        if (updated == null)
        {
            return NotFound();
        }
        
        return Ok(updated);
    }

    /// <summary>
    /// Deletes an encounter.
    /// </summary>
    /// <param name="id">The encounter ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting encounter with ID: {Id}", id);
        var deleted = await _encounterService.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}
