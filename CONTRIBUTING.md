# Contributing to FhirSide

First off, thank you for considering contributing to FhirSide! It's people like you that make FhirSide such a great tool for the healthcare interoperability community.

## Code of Conduct

This project and everyone participating in it is governed by our commitment to creating a welcoming and inclusive environment. By participating, you are expected to uphold this standard.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When you are creating a bug report, please include as many details as possible:

- **Use a clear and descriptive title** for the issue
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples** to demonstrate the steps
- **Describe the behavior you observed** and what you expected
- **Include screenshots** if applicable
- **Include your environment details** (.NET version, OS, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide a detailed description** of the suggested enhancement
- **Explain why this enhancement would be useful**
- **List any additional context** or screenshots

### Pull Requests

1. **Fork the repository** and create your branch from `main`
2. **Follow the coding conventions** used throughout the project
3. **Add tests** for any new functionality
4. **Ensure the test suite passes** by running `dotnet test`
5. **Update documentation** if needed
6. **Write a clear commit message** describing your changes

## Development Setup

### Prerequisites

- .NET 8.0 SDK or later
- Git
- Your favorite IDE (Visual Studio, VS Code, Rider, etc.)

### Getting Started

```bash
# Clone your fork
git clone https://github.com/YOUR_USERNAME/fhirside.git
cd fhirside

# Add upstream remote
git remote add upstream https://github.com/sean-charles-allen/fhirside.git

# Install dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test
```

### Making Changes

1. Create a new branch:
```bash
git checkout -b feature/your-feature-name
```

2. Make your changes and commit:
```bash
git add .
git commit -m "Add description of your changes"
```

3. Push to your fork:
```bash
git push origin feature/your-feature-name
```

4. Open a Pull Request on GitHub

## Coding Guidelines

### C# Style

- Follow the [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use meaningful names for variables, methods, and classes
- Write XML documentation comments for public APIs
- Use async/await for asynchronous operations

### Project Structure

- Place controllers in `FhirSide.Api/Controllers/`
- Place service interfaces and implementations in `FhirSide.Core/Services/`
- Place unit tests in `FhirSide.Api.Tests/`

### Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests when relevant

### Testing

- Write unit tests for new functionality
- Ensure all tests pass before submitting a PR
- Aim for good test coverage

## FHIR Guidelines

When implementing FHIR resources or functionality:

- Follow the [FHIR R4 specification](https://hl7.org/fhir/R4/)
- Use the Firely SDK for FHIR data types and resources
- Ensure responses conform to FHIR JSON format
- Include appropriate status codes and error responses

## Questions?

Feel free to open an issue with your question or reach out to the maintainers.

Thank you for contributing! 🎉
