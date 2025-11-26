using FhirSide.Core.Services;
using Hl7.Fhir.Model;
using Xunit;

namespace FhirSide.Api.Tests;

public class PatientServiceTests
{
    private readonly IPatientService _patientService;

    public PatientServiceTests()
    {
        _patientService = new InMemoryPatientService();
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_ReturnsSeededPatients()
    {
        // Act
        var patients = await _patientService.GetAllAsync();

        // Assert
        Assert.NotNull(patients);
        Assert.True(patients.Any(), "Should have seeded patients");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WithValidId_ReturnsPatient()
    {
        // Arrange
        var expectedId = "1";

        // Act
        var patient = await _patientService.GetByIdAsync(expectedId);

        // Assert
        Assert.NotNull(patient);
        Assert.Equal(expectedId, patient.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = "nonexistent";

        // Act
        var patient = await _patientService.GetByIdAsync(invalidId);

        // Assert
        Assert.Null(patient);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateAsync_AddsNewPatient()
    {
        // Arrange
        var newPatient = new Patient
        {
            Name = new List<HumanName>
            {
                new() { Given = new[] { "Test" }, Family = "Patient" }
            },
            Gender = AdministrativeGender.Other
        };

        // Act
        var created = await _patientService.CreateAsync(newPatient);

        // Assert
        Assert.NotNull(created);
        Assert.NotNull(created.Id);
        Assert.Equal("Test", created.Name.First().Given.First());
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithValidId_UpdatesPatient()
    {
        // Arrange
        var existingId = "1";
        var updatedPatient = new Patient
        {
            Name = new List<HumanName>
            {
                new() { Given = new[] { "Updated" }, Family = "Name" }
            }
        };

        // Act
        var result = await _patientService.UpdateAsync(existingId, updatedPatient);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name.First().Given.First());
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var invalidId = "nonexistent";
        var patient = new Patient();

        // Act
        var result = await _patientService.UpdateAsync(invalidId, patient);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        // Arrange
        var newPatient = await _patientService.CreateAsync(new Patient
        {
            Name = new List<HumanName> { new() { Family = "ToDelete" } }
        });

        // Act
        var result = await _patientService.DeleteAsync(newPatient.Id!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WithInvalidId_ReturnsFalse()
    {
        // Arrange
        var invalidId = "nonexistent";

        // Act
        var result = await _patientService.DeleteAsync(invalidId);

        // Assert
        Assert.False(result);
    }
}
