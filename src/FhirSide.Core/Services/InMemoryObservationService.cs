using System.Collections.Concurrent;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Core.Services;

/// <summary>
/// In-memory implementation of IObservationService for development and testing.
/// </summary>
public class InMemoryObservationService : IObservationService
{
    private readonly ConcurrentDictionary<string, Observation> _observations = new();
    private int _nextId = 1;

    public InMemoryObservationService()
    {
        // Seed with sample data
        SeedSampleData();
    }

    private void SeedSampleData()
    {
        var observation1 = new Observation
        {
            Id = "1",
            Status = ObservationStatus.Final,
            Category = new List<CodeableConcept>
            {
                new()
                {
                    Coding = new List<Coding>
                    {
                        new("http://terminology.hl7.org/CodeSystem/observation-category", "vital-signs", "Vital Signs")
                    }
                }
            },
            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new("http://loinc.org", "8867-4", "Heart rate")
                },
                Text = "Heart rate"
            },
            Subject = new ResourceReference("Patient/1"),
            Effective = new FhirDateTime("2024-01-15T09:30:00Z"),
            Value = new Quantity
            {
                Value = 72,
                Unit = "beats/minute",
                System = "http://unitsofmeasure.org",
                Code = "/min"
            }
        };

        var observation2 = new Observation
        {
            Id = "2",
            Status = ObservationStatus.Final,
            Category = new List<CodeableConcept>
            {
                new()
                {
                    Coding = new List<Coding>
                    {
                        new("http://terminology.hl7.org/CodeSystem/observation-category", "vital-signs", "Vital Signs")
                    }
                }
            },
            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new("http://loinc.org", "8310-5", "Body temperature")
                },
                Text = "Body temperature"
            },
            Subject = new ResourceReference("Patient/1"),
            Effective = new FhirDateTime("2024-01-15T09:30:00Z"),
            Value = new Quantity
            {
                Value = 98.6m,
                Unit = "°F",
                System = "http://unitsofmeasure.org",
                Code = "[degF]"
            }
        };

        var observation3 = new Observation
        {
            Id = "3",
            Status = ObservationStatus.Final,
            Category = new List<CodeableConcept>
            {
                new()
                {
                    Coding = new List<Coding>
                    {
                        new("http://terminology.hl7.org/CodeSystem/observation-category", "vital-signs", "Vital Signs")
                    }
                }
            },
            Code = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new("http://loinc.org", "85354-9", "Blood pressure panel")
                },
                Text = "Blood pressure"
            },
            Subject = new ResourceReference("Patient/2"),
            Effective = new FhirDateTime("2024-02-10T14:45:00Z"),
            Component = new List<Observation.ComponentComponent>
            {
                new()
                {
                    Code = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new("http://loinc.org", "8480-6", "Systolic blood pressure")
                        }
                    },
                    Value = new Quantity { Value = 120, Unit = "mmHg", System = "http://unitsofmeasure.org", Code = "mm[Hg]" }
                },
                new()
                {
                    Code = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new("http://loinc.org", "8462-4", "Diastolic blood pressure")
                        }
                    },
                    Value = new Quantity { Value = 80, Unit = "mmHg", System = "http://unitsofmeasure.org", Code = "mm[Hg]" }
                }
            }
        };

        _observations.TryAdd("1", observation1);
        _observations.TryAdd("2", observation2);
        _observations.TryAdd("3", observation3);
        _nextId = 4;
    }

    public Task<IEnumerable<Observation>> GetAllAsync()
    {
        return Task.FromResult(_observations.Values.AsEnumerable());
    }

    public Task<Observation?> GetByIdAsync(string id)
    {
        _observations.TryGetValue(id, out var observation);
        return Task.FromResult(observation);
    }

    public Task<IEnumerable<Observation>> GetByPatientIdAsync(string patientId)
    {
        var patientRef = $"Patient/{patientId}";
        var observations = _observations.Values
            .Where(o => o.Subject?.Reference == patientRef)
            .ToList();
        return Task.FromResult(observations.AsEnumerable());
    }

    public Task<Observation> CreateAsync(Observation observation)
    {
        var id = Interlocked.Increment(ref _nextId).ToString();
        observation.Id = id;
        _observations.TryAdd(id, observation);
        return Task.FromResult(observation);
    }

    public Task<Observation?> UpdateAsync(string id, Observation observation)
    {
        if (!_observations.ContainsKey(id))
        {
            return Task.FromResult<Observation?>(null);
        }

        observation.Id = id;
        _observations[id] = observation;
        return Task.FromResult<Observation?>(observation);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return Task.FromResult(_observations.TryRemove(id, out _));
    }
}
