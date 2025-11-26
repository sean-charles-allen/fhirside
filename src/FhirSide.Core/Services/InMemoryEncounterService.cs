using System.Collections.Concurrent;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Core.Services;

/// <summary>
/// In-memory implementation of IEncounterService for development and testing.
/// </summary>
public class InMemoryEncounterService : IEncounterService
{
    private readonly ConcurrentDictionary<string, Encounter> _encounters = new();
    private int _nextId = 1;

    public InMemoryEncounterService()
    {
        // Seed with sample data
        SeedSampleData();
    }

    private void SeedSampleData()
    {
        var encounter1 = new Encounter
        {
            Id = "1",
            Status = Encounter.EncounterStatus.Finished,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB", "ambulatory"),
            Subject = new ResourceReference("Patient/1"),
            Period = new Period
            {
                Start = "2024-01-15T09:00:00Z",
                End = "2024-01-15T10:00:00Z"
            },
            ReasonCode = new List<CodeableConcept>
            {
                new()
                {
                    Coding = new List<Coding>
                    {
                        new("http://snomed.info/sct", "185349003", "Encounter for check up")
                    }
                }
            }
        };

        var encounter2 = new Encounter
        {
            Id = "2",
            Status = Encounter.EncounterStatus.InProgress,
            Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "EMER", "emergency"),
            Subject = new ResourceReference("Patient/2"),
            Period = new Period
            {
                Start = "2024-02-10T14:30:00Z"
            }
        };

        _encounters.TryAdd("1", encounter1);
        _encounters.TryAdd("2", encounter2);
        _nextId = 3;
    }

    public Task<IEnumerable<Encounter>> GetAllAsync()
    {
        return Task.FromResult(_encounters.Values.AsEnumerable());
    }

    public Task<Encounter?> GetByIdAsync(string id)
    {
        _encounters.TryGetValue(id, out var encounter);
        return Task.FromResult(encounter);
    }

    public Task<IEnumerable<Encounter>> GetByPatientIdAsync(string patientId)
    {
        var patientRef = $"Patient/{patientId}";
        var encounters = _encounters.Values
            .Where(e => e.Subject?.Reference == patientRef)
            .ToList();
        return Task.FromResult(encounters.AsEnumerable());
    }

    public Task<Encounter> CreateAsync(Encounter encounter)
    {
        var id = Interlocked.Increment(ref _nextId).ToString();
        encounter.Id = id;
        _encounters.TryAdd(id, encounter);
        return Task.FromResult(encounter);
    }

    public Task<Encounter?> UpdateAsync(string id, Encounter encounter)
    {
        if (!_encounters.ContainsKey(id))
        {
            return Task.FromResult<Encounter?>(null);
        }

        encounter.Id = id;
        _encounters[id] = encounter;
        return Task.FromResult<Encounter?>(encounter);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return Task.FromResult(_encounters.TryRemove(id, out _));
    }
}
