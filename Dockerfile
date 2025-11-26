# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY FhirSide.sln .
COPY Directory.Build.props .
COPY src/FhirSide.Api/FhirSide.Api.csproj src/FhirSide.Api/
COPY src/FhirSide.Core/FhirSide.Core.csproj src/FhirSide.Core/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY src/ src/

# Build and publish
RUN dotnet publish src/FhirSide.Api/FhirSide.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r fhirside && useradd -r -g fhirside fhirside

# Copy published application
COPY --from=build /app/publish .

# Set ownership
RUN chown -R fhirside:fhirside /app

# Switch to non-root user
USER fhirside

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "FhirSide.Api.dll"]
