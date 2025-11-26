using System.Collections.Concurrent;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Core.Services;

/// <summary>
/// In-memory implementation of IMedicationRequestService for development and testing.
/// </summary>
public class InMemoryMedicationRequestService : IMedicationRequestService
{
    private readonly ConcurrentDictionary<string, MedicationRequest> _medicationRequests = new();
    private int _nextId = 1;

    public InMemoryMedicationRequestService()
    {
        // Seed with sample data
        SeedSampleData();
    }

    private void SeedSampleData()
    {
        var medicationRequest1 = new MedicationRequest
        {
            Id = "1",
            Status = MedicationRequest.MedicationrequestStatus.Active,
            Intent = MedicationRequest.MedicationRequestIntent.Order,
            Medication = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new("http://www.nlm.nih.gov/research/umls/rxnorm", "197361", "Lisinopril 10 MG Oral Tablet")
                },
                Text = "Lisinopril 10mg"
            },
            Subject = new ResourceReference("Patient/1"),
            AuthoredOn = "2024-01-15",
            Requester = new ResourceReference("Practitioner/1"),
            DosageInstruction = new List<Dosage>
            {
                new()
                {
                    Text = "Take one tablet by mouth once daily",
                    Timing = new Timing
                    {
                        Repeat = new Timing.RepeatComponent
                        {
                            Frequency = 1,
                            Period = 1,
                            PeriodUnit = Timing.UnitsOfTime.D
                        }
                    },
                    Route = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new("http://snomed.info/sct", "26643006", "Oral route")
                        }
                    },
                    DoseAndRate = new List<Dosage.DoseAndRateComponent>
                    {
                        new()
                        {
                            Dose = new Quantity { Value = 1, Unit = "tablet" }
                        }
                    }
                }
            }
        };

        var medicationRequest2 = new MedicationRequest
        {
            Id = "2",
            Status = MedicationRequest.MedicationrequestStatus.Active,
            Intent = MedicationRequest.MedicationRequestIntent.Order,
            Medication = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new("http://www.nlm.nih.gov/research/umls/rxnorm", "860975", "Metformin hydrochloride 500 MG Oral Tablet")
                },
                Text = "Metformin 500mg"
            },
            Subject = new ResourceReference("Patient/2"),
            AuthoredOn = "2024-02-10",
            Requester = new ResourceReference("Practitioner/1"),
            DosageInstruction = new List<Dosage>
            {
                new()
                {
                    Text = "Take one tablet by mouth twice daily with meals",
                    Timing = new Timing
                    {
                        Repeat = new Timing.RepeatComponent
                        {
                            Frequency = 2,
                            Period = 1,
                            PeriodUnit = Timing.UnitsOfTime.D
                        }
                    },
                    Route = new CodeableConcept
                    {
                        Coding = new List<Coding>
                        {
                            new("http://snomed.info/sct", "26643006", "Oral route")
                        }
                    },
                    DoseAndRate = new List<Dosage.DoseAndRateComponent>
                    {
                        new()
                        {
                            Dose = new Quantity { Value = 1, Unit = "tablet" }
                        }
                    }
                }
            }
        };

        _medicationRequests.TryAdd("1", medicationRequest1);
        _medicationRequests.TryAdd("2", medicationRequest2);
        _nextId = 3;
    }

    public Task<IEnumerable<MedicationRequest>> GetAllAsync()
    {
        return Task.FromResult(_medicationRequests.Values.AsEnumerable());
    }

    public Task<MedicationRequest?> GetByIdAsync(string id)
    {
        _medicationRequests.TryGetValue(id, out var medicationRequest);
        return Task.FromResult(medicationRequest);
    }

    public Task<IEnumerable<MedicationRequest>> GetByPatientIdAsync(string patientId)
    {
        var patientRef = $"Patient/{patientId}";
        var medicationRequests = _medicationRequests.Values
            .Where(m => m.Subject?.Reference == patientRef)
            .ToList();
        return Task.FromResult(medicationRequests.AsEnumerable());
    }

    public Task<MedicationRequest> CreateAsync(MedicationRequest medicationRequest)
    {
        var id = Interlocked.Increment(ref _nextId).ToString();
        medicationRequest.Id = id;
        _medicationRequests.TryAdd(id, medicationRequest);
        return Task.FromResult(medicationRequest);
    }

    public Task<MedicationRequest?> UpdateAsync(string id, MedicationRequest medicationRequest)
    {
        if (!_medicationRequests.ContainsKey(id))
        {
            return Task.FromResult<MedicationRequest?>(null);
        }

        medicationRequest.Id = id;
        _medicationRequests[id] = medicationRequest;
        return Task.FromResult<MedicationRequest?>(medicationRequest);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return Task.FromResult(_medicationRequests.TryRemove(id, out _));
    }
}
