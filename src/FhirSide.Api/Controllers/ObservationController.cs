using FhirSide.Core.Services;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;

namespace FhirSide.Api.Controllers;

/// <summary>
/// Controller for FHIR Observation resources.
/// </summary>
[ApiController]
[Route("fhir/[controller]")]
[Produces("application/fhir+json")]
public class ObservationController : ControllerBase
{
    private readonly IObservationService _observationService;
    private readonly ILogger<ObservationController> _logger;

    public ObservationController(IObservationService observationService, ILogger<ObservationController> logger)
    {
        _observationService = observationService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all observations.
    /// </summary>
    /// <returns>A Bundle containing all observations.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetAll()
    {
        _logger.LogInformation("Getting all observations");
        var observations = await _observationService.GetAllAsync();
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = observations.Count(),
            Entry = observations.Select(o => new Bundle.EntryComponent
            {
                Resource = o,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/Observation/{o.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Gets an observation by ID.
    /// </summary>
    /// <param name="id">The observation ID.</param>
    /// <returns>The observation resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Observation>> GetById(string id)
    {
        _logger.LogInformation("Getting observation with ID: {Id}", id);
        var observation = await _observationService.GetByIdAsync(id);
        
        if (observation == null)
        {
            return NotFound();
        }
        
        return Ok(observation);
    }

    /// <summary>
    /// Gets observations for a specific patient.
    /// </summary>
    /// <param name="patientId">The patient ID.</param>
    /// <returns>A Bundle containing all observations for the patient.</returns>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetByPatient(string patientId)
    {
        _logger.LogInformation("Getting observations for patient: {PatientId}", patientId);
        var observations = await _observationService.GetByPatientIdAsync(patientId);
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = observations.Count(),
            Entry = observations.Select(o => new Bundle.EntryComponent
            {
                Resource = o,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/Observation/{o.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Creates a new observation.
    /// </summary>
    /// <param name="observation">The observation to create.</param>
    /// <returns>The created observation.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Observation>> Create([FromBody] Observation observation)
    {
        _logger.LogInformation("Creating new observation");
        var created = await _observationService.CreateAsync(observation);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing observation.
    /// </summary>
    /// <param name="id">The observation ID.</param>
    /// <param name="observation">The updated observation.</param>
    /// <returns>The updated observation.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Observation>> Update(string id, [FromBody] Observation observation)
    {
        _logger.LogInformation("Updating observation with ID: {Id}", id);
        var updated = await _observationService.UpdateAsync(id, observation);
        
        if (updated == null)
        {
            return NotFound();
        }
        
        return Ok(updated);
    }

    /// <summary>
    /// Deletes an observation.
    /// </summary>
    /// <param name="id">The observation ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting observation with ID: {Id}", id);
        var deleted = await _observationService.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}
