using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace FhirSide.Api.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async System.Threading.Tasks.Task HealthCheck_ReturnsHealthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPatients_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Patient");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("searchset", content);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPatientById_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Patient/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPatientById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Patient/nonexistent");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetEncounters_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Encounter");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetObservations_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Observation");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetMedicationRequests_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/MedicationRequest");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPatientById_ReturnsFhirCompliantJson()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Patient/1");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Parse JSON and verify FHIR-compliant structure
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        
        // FHIR resources must have a resourceType property
        Assert.True(root.TryGetProperty("resourceType", out var resourceType), "Response should contain 'resourceType' property");
        Assert.Equal("Patient", resourceType.GetString());
        
        // FHIR resources should have an id
        Assert.True(root.TryGetProperty("id", out var id), "Response should contain 'id' property");
        Assert.Equal("1", id.GetString());
        
        // Should not contain .NET-specific properties like TypeName
        Assert.False(root.TryGetProperty("TypeName", out _), "Response should not contain .NET-specific 'TypeName' property");
        Assert.False(root.TryGetProperty("ElementIdElement", out _), "Response should not contain .NET-specific 'ElementIdElement' property");
    }

    [Fact]
    public async System.Threading.Tasks.Task GetPatients_ReturnsFhirCompliantBundle()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Patient");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Parse JSON and verify FHIR Bundle structure
        var json = JsonDocument.Parse(content);
        var root = json.RootElement;
        
        // FHIR Bundle must have resourceType = "Bundle"
        Assert.True(root.TryGetProperty("resourceType", out var resourceType), "Response should contain 'resourceType' property");
        Assert.Equal("Bundle", resourceType.GetString());
        
        // FHIR Bundle must have a type
        Assert.True(root.TryGetProperty("type", out var bundleType), "Response should contain 'type' property");
        Assert.Equal("searchset", bundleType.GetString());
        
        // Should not contain .NET-specific properties
        Assert.False(root.TryGetProperty("TypeName", out _), "Response should not contain .NET-specific 'TypeName' property");
    }
}
