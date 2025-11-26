using System.Collections.Concurrent;
using Hl7.Fhir.Model;
using Task = System.Threading.Tasks.Task;

namespace FhirSide.Core.Services;

/// <summary>
/// In-memory implementation of IPatientService for development and testing.
/// </summary>
public class InMemoryPatientService : IPatientService
{
    private readonly ConcurrentDictionary<string, Patient> _patients = new();
    private int _nextId = 1;

    public InMemoryPatientService()
    {
        // Seed with sample data
        SeedSampleData();
    }

    private void SeedSampleData()
    {
        var patient1 = new Patient
        {
            Id = "1",
            Name = new List<HumanName>
            {
                new() { Given = new[] { "John" }, Family = "Doe" }
            },
            Gender = AdministrativeGender.Male,
            BirthDate = "1980-01-15",
            Address = new List<Address>
            {
                new()
                {
                    Line = new[] { "123 Main Street" },
                    City = "Boston",
                    State = "MA",
                    PostalCode = "02101",
                    Country = "USA"
                }
            }
        };

        var patient2 = new Patient
        {
            Id = "2",
            Name = new List<HumanName>
            {
                new() { Given = new[] { "Jane" }, Family = "Smith" }
            },
            Gender = AdministrativeGender.Female,
            BirthDate = "1985-07-22",
            Address = new List<Address>
            {
                new()
                {
                    Line = new[] { "456 Oak Avenue" },
                    City = "Cambridge",
                    State = "MA",
                    PostalCode = "02139",
                    Country = "USA"
                }
            }
        };

        _patients.TryAdd("1", patient1);
        _patients.TryAdd("2", patient2);
        _nextId = 3;
    }

    public Task<IEnumerable<Patient>> GetAllAsync()
    {
        return Task.FromResult(_patients.Values.AsEnumerable());
    }

    public Task<Patient?> GetByIdAsync(string id)
    {
        _patients.TryGetValue(id, out var patient);
        return Task.FromResult(patient);
    }

    public Task<Patient> CreateAsync(Patient patient)
    {
        var id = Interlocked.Increment(ref _nextId).ToString();
        patient.Id = id;
        _patients.TryAdd(id, patient);
        return Task.FromResult(patient);
    }

    public Task<Patient?> UpdateAsync(string id, Patient patient)
    {
        if (!_patients.ContainsKey(id))
        {
            return Task.FromResult<Patient?>(null);
        }

        patient.Id = id;
        _patients[id] = patient;
        return Task.FromResult<Patient?>(patient);
    }

    public Task<bool> DeleteAsync(string id)
    {
        return Task.FromResult(_patients.TryRemove(id, out _));
    }
}
