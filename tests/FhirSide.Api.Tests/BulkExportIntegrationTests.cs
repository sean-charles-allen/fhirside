using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FhirSide.Api.Tests;

public class BulkExportIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BulkExportIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private static string? GetContentLocationHeader(HttpResponseMessage response)
    {
        // Content-Location can appear in response headers or content headers
        if (response.Headers.TryGetValues("Content-Location", out var values))
        {
            return values.FirstOrDefault();
        }
        if (response.Content.Headers.TryGetValues("Content-Location", out var contentValues))
        {
            return contentValues.FirstOrDefault();
        }
        return null;
    }

    [Fact]
    public async System.Threading.Tasks.Task SystemExport_Returns202Accepted_WithContentLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/$export");

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var contentLocation = GetContentLocationHeader(response);
        Assert.NotNull(contentLocation);
        Assert.Contains("$export-status", contentLocation);
    }

    [Fact]
    public async System.Threading.Tasks.Task PatientExport_Returns202Accepted_WithContentLocationHeader()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/Patient/$export");

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var contentLocation = GetContentLocationHeader(response);
        Assert.NotNull(contentLocation);
        Assert.Contains("$export-status", contentLocation);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExportStatus_WithValidJobId_ReturnsCompleted()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Start export
        var exportResponse = await client.GetAsync("/fhir/$export");
        var contentLocation = GetContentLocationHeader(exportResponse);
        Assert.NotNull(contentLocation);

        // Wait for export to complete (in test environment it should be fast)
        await System.Threading.Tasks.Task.Delay(500);

        // Act
        var statusResponse = await client.GetAsync(contentLocation);

        // Assert
        Assert.Equal(HttpStatusCode.OK, statusResponse.StatusCode);
        var content = await statusResponse.Content.ReadAsStringAsync();
        Assert.Contains("output", content);
        Assert.Contains("transactionTime", content);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExportStatus_WithInvalidJobId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/fhir/$export-status/nonexistent-job-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExportDownload_WithCompletedExport_ReturnsNdjsonContent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Start export
        var exportResponse = await client.GetAsync("/fhir/$export");
        var contentLocation = GetContentLocationHeader(exportResponse);
        Assert.NotNull(contentLocation);

        // Wait for export to complete
        await System.Threading.Tasks.Task.Delay(500);

        // Get status to find download URLs
        var statusResponse = await client.GetAsync(contentLocation);
        var statusContent = await statusResponse.Content.ReadAsStringAsync();

        // Extract job ID from content location
        var jobId = contentLocation.Split('/').Last();

        // Act - Download Patient file
        var downloadResponse = await client.GetAsync($"/fhir/$export-download/{jobId}/Patient.ndjson");

        // Assert
        Assert.Equal(HttpStatusCode.OK, downloadResponse.StatusCode);
        var contentType = downloadResponse.Content.Headers.ContentType?.MediaType;
        Assert.Equal("application/ndjson", contentType);

        var ndjsonContent = await downloadResponse.Content.ReadAsStringAsync();
        Assert.NotEmpty(ndjsonContent);
        // NDJSON should contain valid JSON lines
        var lines = ndjsonContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.True(lines.Length > 0);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExportDownload_WithInvalidFile_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Start export
        var exportResponse = await client.GetAsync("/fhir/$export");
        var contentLocation = GetContentLocationHeader(exportResponse);
        Assert.NotNull(contentLocation);
        var jobId = contentLocation.Split('/').Last();

        // Wait for export to complete
        await System.Threading.Tasks.Task.Delay(500);

        // Act
        var response = await client.GetAsync($"/fhir/$export-download/{jobId}/NonExistent.ndjson");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteExportJob_WithValidJobId_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Start export
        var exportResponse = await client.GetAsync("/fhir/$export");
        var contentLocation = GetContentLocationHeader(exportResponse);
        Assert.NotNull(contentLocation);
        var jobId = contentLocation.Split('/').Last();

        // Wait for export to complete
        await System.Threading.Tasks.Task.Delay(500);

        // Act
        var deleteResponse = await client.DeleteAsync($"/fhir/$export-status/{jobId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify job is deleted
        var statusResponse = await client.GetAsync(contentLocation);
        Assert.Equal(HttpStatusCode.NotFound, statusResponse.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteExportJob_WithInvalidJobId_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync("/fhir/$export-status/nonexistent-job-id");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExportStatus_CompletedExport_ContainsAllResourceTypes()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Start export
        var exportResponse = await client.GetAsync("/fhir/$export");
        var contentLocation = GetContentLocationHeader(exportResponse);
        Assert.NotNull(contentLocation);

        // Wait for export to complete
        await System.Threading.Tasks.Task.Delay(500);

        // Act
        var statusResponse = await client.GetAsync(contentLocation);
        var content = await statusResponse.Content.ReadAsStringAsync();

        // Assert - should contain all supported resource types
        Assert.Contains("Patient", content);
        Assert.Contains("Encounter", content);
        Assert.Contains("Observation", content);
        Assert.Contains("MedicationRequest", content);
    }
}
