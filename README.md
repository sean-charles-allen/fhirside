# FhirSide

A FHIR R4 compliant sandbox API for healthcare data interoperability. FhirSide provides a simulated FHIR-compliant healthcare system with synthetic patient data for learning, testing, and development purposes.

## 🎯 Project Goals

- **Educational Resource**: Help developers learn FHIR (Fast Healthcare Interoperability Resources) standards
- **Development Sandbox**: Provide a safe environment for testing FHIR integrations
- **Clean Architecture**: Demonstrate .NET best practices and clean architecture principles
- **FHIR Compliance**: Implement FHIR R4 standard using the Firely SDK

## ✨ Features

### Current Features
- ✅ RESTful API endpoints for core FHIR resources
- ✅ In-memory data storage with sample synthetic data
- ✅ Swagger/OpenAPI documentation
- ✅ FHIR R4 compliant resource models
- ✅ Docker support for containerized deployment

### Supported FHIR Resources
| Resource | Status | Description |
|----------|--------|-------------|
| Patient | ✅ Implemented | Demographic and administrative information about a patient |
| Encounter | ✅ Implemented | Healthcare events involving a patient |
| Observation | ✅ Implemented | Measurements and simple assertions (vital signs, lab results) |
| MedicationRequest | ✅ Implemented | Orders for medications |

### Planned Features
- [ ] SMART on FHIR authentication
- [ ] Persistent database storage (PostgreSQL/SQLite)
- [ ] Additional FHIR resources (Condition, Procedure, DiagnosticReport)
- [ ] FHIR search parameters and modifiers
- [ ] Bulk data export (FHIR Bulk Data Access)
- [ ] Subscription-based notifications

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Docker](https://www.docker.com/get-started) (optional, for containerized deployment)

### Installation

1. Clone the repository:
```bash
git clone https://github.com/sean-charles-allen/fhirside.git
cd fhirside
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the solution:
```bash
dotnet build
```

4. Run the API:
```bash
cd src/FhirSide.Api
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

### Using Docker

Build and run using Docker:
```bash
docker build -t fhirside .
docker run -p 8080:8080 fhirside
```

Or use Docker Compose:
```bash
docker-compose up
```

## 📖 API Documentation

Once the application is running, access the interactive Swagger documentation at:
- **Development**: `https://localhost:5001/` or `http://localhost:5000/`
- **Docker**: `http://localhost:8080/`

### API Endpoints

#### Patient Resource
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/fhir/Patient` | Get all patients (as Bundle) |
| GET | `/fhir/Patient/{id}` | Get patient by ID |
| POST | `/fhir/Patient` | Create a new patient |
| PUT | `/fhir/Patient/{id}` | Update an existing patient |
| DELETE | `/fhir/Patient/{id}` | Delete a patient |

#### Encounter Resource
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/fhir/Encounter` | Get all encounters (as Bundle) |
| GET | `/fhir/Encounter/{id}` | Get encounter by ID |
| GET | `/fhir/Encounter/patient/{patientId}` | Get encounters for a patient |
| POST | `/fhir/Encounter` | Create a new encounter |
| PUT | `/fhir/Encounter/{id}` | Update an existing encounter |
| DELETE | `/fhir/Encounter/{id}` | Delete an encounter |

#### Observation Resource
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/fhir/Observation` | Get all observations (as Bundle) |
| GET | `/fhir/Observation/{id}` | Get observation by ID |
| GET | `/fhir/Observation/patient/{patientId}` | Get observations for a patient |
| POST | `/fhir/Observation` | Create a new observation |
| PUT | `/fhir/Observation/{id}` | Update an existing observation |
| DELETE | `/fhir/Observation/{id}` | Delete an observation |

#### MedicationRequest Resource
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/fhir/MedicationRequest` | Get all medication requests (as Bundle) |
| GET | `/fhir/MedicationRequest/{id}` | Get medication request by ID |
| GET | `/fhir/MedicationRequest/patient/{patientId}` | Get medication requests for a patient |
| POST | `/fhir/MedicationRequest` | Create a new medication request |
| PUT | `/fhir/MedicationRequest/{id}` | Update an existing medication request |
| DELETE | `/fhir/MedicationRequest/{id}` | Delete a medication request |

## 🏗️ Project Structure

```
fhirside/
├── src/
│   ├── FhirSide.Api/           # ASP.NET Core Web API project
│   │   ├── Controllers/        # API controllers for FHIR resources
│   │   └── Program.cs          # Application entry point
│   └── FhirSide.Core/          # Core domain library
│       └── Services/           # Business logic and service interfaces
├── tests/
│   └── FhirSide.Api.Tests/     # Unit and integration tests
├── FhirSide.sln               # Solution file
├── Directory.Build.props      # Shared build properties
├── Dockerfile                 # Docker configuration
├── docker-compose.yml         # Docker Compose configuration
└── README.md                  # This file
```

## 🧪 Running Tests

```bash
dotnet test
```

## 🔧 Configuration

### Application Settings

The application can be configured via `appsettings.json` or environment variables:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 🤝 Contributing

Contributions are welcome! Please read our [Contributing Guidelines](CONTRIBUTING.md) before submitting a pull request.

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📚 Resources

- [FHIR R4 Specification](https://hl7.org/fhir/R4/)
- [Firely .NET SDK](https://github.com/FirelyTeam/firely-net-sdk)
- [SMART on FHIR](https://smarthealthit.org/)
- [HL7 International](https://www.hl7.org/)

## 📞 Support

If you have questions or need help, please:
- Open an [issue](https://github.com/sean-charles-allen/fhirside/issues)
- Check existing issues for answers

---

Made with ❤️ for the healthcare interoperability community