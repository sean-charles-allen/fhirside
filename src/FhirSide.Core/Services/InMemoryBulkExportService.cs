using System.Collections.Concurrent;
using System.Text;
using FhirSide.Core.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Core.Services;

/// <summary>
/// In-memory implementation of IBulkExportService for development and testing.
/// </summary>
public class InMemoryBulkExportService : IBulkExportService
{
    private readonly IPatientService _patientService;
    private readonly IEncounterService _encounterService;
    private readonly IObservationService _observationService;
    private readonly IMedicationRequestService _medicationRequestService;
    private readonly ConcurrentDictionary<string, BulkExportJob> _jobs = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _exportFiles = new();
    private readonly FhirJsonSerializer _serializer;

    public InMemoryBulkExportService(
        IPatientService patientService,
        IEncounterService encounterService,
        IObservationService observationService,
        IMedicationRequestService medicationRequestService)
    {
        _patientService = patientService;
        _encounterService = encounterService;
        _observationService = observationService;
        _medicationRequestService = medicationRequestService;
        _serializer = new FhirJsonSerializer();
    }

    public async Task<BulkExportJob> StartSystemExportAsync(string baseUrl)
    {
        var job = CreateJob("system", baseUrl);
        _jobs.TryAdd(job.Id, job);
        _exportFiles.TryAdd(job.Id, new ConcurrentDictionary<string, string>());

        // Execute export asynchronously
        _ = ExecuteExportAsync(job, includeAllResources: true);

        return job;
    }

    public async Task<BulkExportJob> StartPatientExportAsync(string baseUrl)
    {
        var job = CreateJob("patient", baseUrl);
        _jobs.TryAdd(job.Id, job);
        _exportFiles.TryAdd(job.Id, new ConcurrentDictionary<string, string>());

        // Execute export asynchronously
        _ = ExecuteExportAsync(job, includeAllResources: true);

        return job;
    }

    public Task<BulkExportJob?> GetJobStatusAsync(string jobId)
    {
        _jobs.TryGetValue(jobId, out var job);
        return Task.FromResult(job);
    }

    public Task<string?> GetExportFileAsync(string jobId, string fileName)
    {
        if (!_exportFiles.TryGetValue(jobId, out var files))
        {
            return Task.FromResult<string?>(null);
        }

        files.TryGetValue(fileName, out var content);
        return Task.FromResult(content);
    }

    public Task<bool> DeleteJobAsync(string jobId)
    {
        var jobRemoved = _jobs.TryRemove(jobId, out _);
        _exportFiles.TryRemove(jobId, out _);
        return Task.FromResult(jobRemoved);
    }

    private static BulkExportJob CreateJob(string exportType, string baseUrl)
    {
        return new BulkExportJob
        {
            Id = Guid.NewGuid().ToString("N"),
            Status = BulkExportStatus.InProgress,
            ExportType = exportType,
            RequestedAt = DateTime.UtcNow,
            BaseUrl = baseUrl
        };
    }

    private async Task ExecuteExportAsync(BulkExportJob job, bool includeAllResources)
    {
        try
        {
            var files = _exportFiles[job.Id];

            // Export Patients
            var patients = await _patientService.GetAllAsync();
            var patientList = patients.ToList();
            if (patientList.Count > 0)
            {
                var patientFileName = "Patient.ndjson";
                var patientContent = SerializeToNdjson(patientList);
                files.TryAdd(patientFileName, patientContent);
                job.Output.Add(new BulkExportOutput
                {
                    Type = "Patient",
                    FileName = patientFileName,
                    Url = $"{job.BaseUrl}/fhir/$export-download/{job.Id}/{patientFileName}",
                    Count = patientList.Count
                });
            }

            // Export Encounters
            var encounters = await _encounterService.GetAllAsync();
            var encounterList = encounters.ToList();
            if (encounterList.Count > 0)
            {
                var encounterFileName = "Encounter.ndjson";
                var encounterContent = SerializeToNdjson(encounterList);
                files.TryAdd(encounterFileName, encounterContent);
                job.Output.Add(new BulkExportOutput
                {
                    Type = "Encounter",
                    FileName = encounterFileName,
                    Url = $"{job.BaseUrl}/fhir/$export-download/{job.Id}/{encounterFileName}",
                    Count = encounterList.Count
                });
            }

            // Export Observations
            var observations = await _observationService.GetAllAsync();
            var observationList = observations.ToList();
            if (observationList.Count > 0)
            {
                var observationFileName = "Observation.ndjson";
                var observationContent = SerializeToNdjson(observationList);
                files.TryAdd(observationFileName, observationContent);
                job.Output.Add(new BulkExportOutput
                {
                    Type = "Observation",
                    FileName = observationFileName,
                    Url = $"{job.BaseUrl}/fhir/$export-download/{job.Id}/{observationFileName}",
                    Count = observationList.Count
                });
            }

            // Export MedicationRequests
            var medicationRequests = await _medicationRequestService.GetAllAsync();
            var medicationRequestList = medicationRequests.ToList();
            if (medicationRequestList.Count > 0)
            {
                var medicationRequestFileName = "MedicationRequest.ndjson";
                var medicationRequestContent = SerializeToNdjson(medicationRequestList);
                files.TryAdd(medicationRequestFileName, medicationRequestContent);
                job.Output.Add(new BulkExportOutput
                {
                    Type = "MedicationRequest",
                    FileName = medicationRequestFileName,
                    Url = $"{job.BaseUrl}/fhir/$export-download/{job.Id}/{medicationRequestFileName}",
                    Count = medicationRequestList.Count
                });
            }

            job.Status = BulkExportStatus.Completed;
            job.CompletedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            job.Status = BulkExportStatus.Failed;
            job.ErrorMessage = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
        }
    }

    private string SerializeToNdjson<T>(IEnumerable<T> resources) where T : Resource
    {
        var sb = new StringBuilder();
        foreach (var resource in resources)
        {
            var json = _serializer.SerializeToString(resource);
            sb.AppendLine(json);
        }
        return sb.ToString().TrimEnd();
    }
}
