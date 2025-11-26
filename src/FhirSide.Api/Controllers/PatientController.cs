using FhirSide.Core.Services;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace FhirSide.Api.Controllers;

/// <summary>
/// Controller for FHIR Patient resources.
/// </summary>
[ApiController]
[Route("fhir/[controller]")]
[Produces("application/fhir+json")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientController> _logger;

    public PatientController(IPatientService patientService, ILogger<PatientController> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all patients.
    /// </summary>
    /// <returns>A Bundle containing all patients.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Bundle>> GetAll()
    {
        _logger.LogInformation("Getting all patients");
        var patients = await _patientService.GetAllAsync();
        
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Searchset,
            Total = patients.Count(),
            Entry = patients.Select(p => new Bundle.EntryComponent
            {
                Resource = p,
                FullUrl = $"{Request.Scheme}://{Request.Host}/fhir/Patient/{p.Id}"
            }).ToList()
        };
        
        return Ok(bundle);
    }

    /// <summary>
    /// Gets a patient by ID.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <returns>The patient resource.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Patient>> GetById(string id)
    {
        _logger.LogInformation("Getting patient with ID: {Id}", id);
        var patient = await _patientService.GetByIdAsync(id);
        
        if (patient == null)
        {
            return NotFound();
        }
        
        return Ok(patient);
    }

    /// <summary>
    /// Creates a new patient.
    /// </summary>
    /// <param name="patient">The patient to create.</param>
    /// <returns>The created patient.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Patient>> Create([FromBody] Patient patient)
    {
        _logger.LogInformation("Creating new patient");
        var created = await _patientService.CreateAsync(patient);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing patient.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <param name="patient">The updated patient.</param>
    /// <returns>The updated patient.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Patient>> Update(string id, [FromBody] Patient patient)
    {
        _logger.LogInformation("Updating patient with ID: {Id}", id);
        var updated = await _patientService.UpdateAsync(id, patient);
        
        if (updated == null)
        {
            return NotFound();
        }
        
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a patient.
    /// </summary>
    /// <param name="id">The patient ID.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogInformation("Deleting patient with ID: {Id}", id);
        var deleted = await _patientService.DeleteAsync(id);
        
        if (!deleted)
        {
            return NotFound();
        }
        
        return NoContent();
    }
}
