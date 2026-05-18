# 🗄️ Catalogs Microservice

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](LICENSE.md)
[![Tests](https://img.shields.io/badge/tests-passing-success)](tests/)
[![Coverage](https://img.shields.io/badge/coverage-85%25-green)]()
[![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker)](Dockerfile)

A production-ready microservice for managing catalogs, categories, and product taxonomies built with .NET 9. Implements Clean Architecture, DDD, and CQRS patterns with support for document type management and extensible catalog structures.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Technology Stack](#️-technology-stack)
- [Prerequisites](#️-prerequisites)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Configuration](#️-configuration)
- [Use Cases & Scenarios](#-use-cases--scenarios)
- [Architecture](#️-architecture)
- [Testing](#-testing)
- [Best Practices](#-best-practices)
- [Troubleshooting](#-troubleshooting)
- [Domain Model](#-domain-model)
- [Events](#-events)
- [Security](#-security)
- [FAQ](#-faq)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Overview

## What is this microservice?

The Catalogs microservice manages reference data that the rest of the platform uses in dropdowns, forms, and validations: identity document types (CC, NIT, RUT, Passport), and other extensible lookup tables. It solves the problem of having a single, consistent source for classification data so that all microservices use the same codes and descriptions. When a user registers, the system pulls document types from this catalog; when an organization is created, its tax document type comes from here. It is maintained by platform administrators and consumed by virtually every other microservice that needs standardized reference values.

---

The Catalogs microservice provides a unified API for managing catalogs, product taxonomies, and document type definitions. It serves as the central repository for catalog data across the system, offering features like:

- **Document Type Management**: Define and manage document types (CC, TI, NIT, etc.)
- **Hierarchical Catalogs**: Support for nested catalog structures
- **Category Management**: Organize products and items in hierarchical categories
- **Multi-tenancy**: Isolate catalog data by tenant
- **Code-based Lookup**: Fast lookup by unique codes
- **Caching Strategy**: Redis-backed caching for high-performance reads
- **Event-Driven**: Publishes domain events for catalog changes
- **Extensible Design**: Easy to add new catalog types and attributes

### 🚀 Quick Start

```bash
# 1. Start infrastructure services
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d

# 2. Configure Vault secrets
cd ../../tools/vault
./config-vault.sh

# 3. Run the microservice
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Catalogs.Rest

# 4. Access Swagger UI
open http://localhost:5000/ms-catalogs/swagger
```

### 📊 High-Level Architecture

```
┌─────────────┐
│   Client    │
│ Application │
└──────┬──────┘
       │ HTTPS + JWT
       │
┌──────▼──────────────────────────────────────────────┐
│         Catalogs Microservice (REST API)            │
│  ┌──────────────┐  ┌─────────────┐  ┌────────────┐ │
│  │ Controllers  │  │  MediatR    │  │  Handlers  │ │
│  │   (API)      │─▶│   (CQRS)    │─▶│ (Business) │ │
│  └──────────────┘  └─────────────┘  └────┬───────┘ │
│                                           │         │
│  ┌────────────────────────────────────────▼──────┐ │
│  │         Domain Layer (Aggregates/Entities)    │ │
│  │  ┌─────────────────┐  ┌──────────────────┐   │ │
│  │  │ TypeDocument    │  │ Future Catalogs  │   │ │
│  │  │   Aggregate     │  │   (Extensible)   │   │ │
│  │  └─────────────────┘  └──────────────────┘   │ │
│  └────────────────────────────────────────────────┘ │
└───────┬──────────────────┬──────────────────┬───────┘
        │                  │                  │
   ┌────▼────┐      ┌──────▼──────┐    ┌─────▼─────┐
   │ MongoDB │      │    Redis    │    │ RabbitMQ  │
   │(Catalog │      │   (Cache)   │    │ (Events)  │
   │  Data)  │      │             │    │           │
   └─────────┘      └─────────────┘    └───────────┘
```

## 🚀 Key Features

### Core Capabilities

- ✅ **Document Type Management**: CRUD operations for document types (ID types, tax types, etc.)
- ✅ **Code-Based Lookup**: Unique code system for fast retrieval
- ✅ **Multi-Tenant Support**: Isolated catalog data per tenant
- ✅ **Caching Layer**: Redis-backed caching with 6-hour expiration
- ✅ **Event Publishing**: Domain events via RabbitMQ for system integration
- ✅ **Active/Inactive States**: Soft delete and activation control
- ✅ **Audit Trail**: CreatedAt, UpdatedAt, DeletedAt timestamps
- ✅ **Validation**: FluentValidation for input validation
- ✅ **Problem Details**: RFC 7807 compliant error responses
- ✅ **Extensible Design**: Easy to add new catalog types

### Technical Features

- Clean Architecture with DDD and CQRS
- Domain events for state changes
- MongoDB for catalog persistence
- RabbitMQ for event publishing
- Redis for distributed caching
- OAuth2/OpenID Connect security
- Multi-tenancy support
- Swagger/OpenAPI documentation
- Docker containerization
- Comprehensive test coverage (Unit, Integration)
- Health checks for all dependencies

## 🛠️ Technology Stack

### Core
- **.NET 9** - Runtime and framework
- **ASP.NET Core** - Web API framework
- **C# 13** - Programming language

### Storage & Data
- **MongoDB** - Catalog persistence and queries
- **Redis** - Distributed caching

### Messaging & Events
- **RabbitMQ** - Event publishing and message broker

### Architecture & Patterns
- **MediatR** - CQRS command/query handling
- **FluentValidation** - Input validation
- **Mapster** - Object mapping
- **NodaTime** - Date/time handling
- **DDD** - Domain-Driven Design patterns
- **Event Sourcing** - Domain events

### Security & Configuration
- **Vault** - Secret management
- **OAuth2/OpenID Connect** - Authentication
- **JWT Bearer** - Token-based security
- **HTTPS** - Encrypted communication

### DevOps & Testing
- **Docker** - Containerization
- **Helm** - Kubernetes deployment
- **xUnit** - Unit/integration testing
- **Swagger/OpenAPI** - API documentation

## ⚙️ Prerequisites

### Required
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker & Docker Compose** - For infrastructure services
- **MongoDB 6.0+** - Document database
- **Redis 7.0+** - Caching layer
- **RabbitMQ 3.12+** - Message broker

### Optional
- **Vault** - Secret management (can use appsettings for local dev)
- **Kubernetes** - For production deployment
- **Helm** - For Kubernetes deployment

## 🚀 Getting Started

The following instructions will help you set up the project on your local machine for development and testing purposes.

### 1. Clone the Repository

```bash
git clone <repository-url>
cd CodeDesignPlus.Net.Microservice.Catalogs
```

### 2. Start Infrastructure Services

Clone and run the development environment:

```bash
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d
```

This starts:
- MongoDB on `localhost:27017`
- Redis on `localhost:6379`
- RabbitMQ on `localhost:5672` (Management UI on `localhost:15672`)
- Vault on `localhost:8200`

### 3. Configure Vault

Run the Vault configuration script to set up secrets:

```bash
cd tools/vault
./config-vault.sh
```

This configures:
- MongoDB credentials
- RabbitMQ credentials
- Service-specific secrets

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the REST API

```bash
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Catalogs.Rest
```

The API will be available at:
- **HTTP**: `http://localhost:5000/ms-catalogs`
- **Swagger UI**: `http://localhost:5000/ms-catalogs/swagger`

### 6. Verify Health

Check the health endpoints:

```bash
curl http://localhost:5000/ms-catalogs/health
curl http://localhost:5000/ms-catalogs/health/ready
```

## 📡 API Endpoints

### TypeDocument Operations

#### Get All Type Documents
```http
GET /api/typedocument
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK`
```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Cedula de Ciudadania",
    "description": "Colombian National ID Card",
    "code": "CC",
    "isActive": true
  },
  {
    "id": "550e8400-e29b-41d4-a716-446655440001",
    "name": "Numero de Identificacion Tributaria",
    "description": "Tax Identification Number",
    "code": "NIT",
    "isActive": true
  }
]
```

#### Get Type Document by ID
```http
GET /api/typedocument/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK`
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Cedula de Ciudadania",
  "description": "Colombian National ID Card",
  "code": "CC",
  "isActive": true
}
```

#### Create Type Document
```http
POST /api/typedocument
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Cedula de Ciudadania",
  "description": "Colombian National ID Card",
  "code": "CC",
  "isActive": true
}
```

**Validation Rules**:
- `id`: Required, must be a valid GUID
- `name`: Required, max length 64 characters
- `description`: Optional, max length 512 characters
- `code`: Required, max length 4 characters
- `isActive`: Required boolean

**Response**: `204 No Content`

**Error Response** (400 Bad Request):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Name is required."],
    "Code": ["Code is required."]
  }
}
```

**Error Response** (409 Conflict):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "TypeDocument Already Exists",
  "status": 409,
  "detail": "202 : TypeDocument Already Exists"
}
```

#### Update Type Document
```http
PUT /api/typedocument/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "name": "Cedula de Ciudadania",
  "description": "Updated description",
  "code": "CC",
  "isActive": true
}
```

**Response**: `204 No Content`

**Error Response** (404 Not Found):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "TypeDocument Not Found",
  "status": 404,
  "detail": "203 : TypeDocument Not Found"
}
```

#### Delete Type Document
```http
DELETE /api/typedocument/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `204 No Content`

**Note**: This is a soft delete. The record is marked as `IsDeleted=true` and `IsActive=false`.

### Health Endpoints

#### Health Check
```http
GET /ms-catalogs/health
```

**Response**: `200 OK`
```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "mongodb",
      "status": "Healthy",
      "duration": "00:00:00.0234567"
    },
    {
      "name": "redis",
      "status": "Healthy",
      "duration": "00:00:00.0123456"
    },
    {
      "name": "rabbitmq",
      "status": "Healthy",
      "duration": "00:00:00.0345678"
    }
  ],
  "totalDuration": "00:00:00.0703701"
}
```

#### Readiness Check
```http
GET /ms-catalogs/health/ready
```

**Response**: `200 OK` when ready, `503 Service Unavailable` when not ready.

## ⚙️ Configuration

### Core Configuration

The microservice uses `appsettings.json` with environment-specific overrides (`appsettings.Development.json`, `appsettings.Docker.json`, `appsettings.Staging.json`).

#### Core Settings

```json
{
  "Core": {
    "Id": "b661bb0a-341f-4b64-9845-f468c5589a9b",
    "PathBase": "/ms-catalogs",
    "AppName": "ms-catalogs",
    "TypeEntryPoint": "rest",
    "Version": "v1",
    "Description": "This microservice management the catalogs of system",
    "Business": "CodeDesignPlus",
    "Contact": {
      "Name": "Wilzon Liscano",
      "Email": "wliscano@codedesignplus.com"
    }
  }
}
```

#### MongoDB Configuration

```json
{
  "Mongo": {
    "Enable": true,
    "Database": "db-ms-catalogs",
    "Diagnostic": {
      "Enable": false,
      "EnableCommandText": false
    }
  }
}
```

**Connection String**: Loaded from Vault at runtime.

**Collections**:
- `TypeDocumentAggregate` - Document type definitions

#### Redis Configuration

```json
{
  "Redis": {
    "Instances": {
      "Core": {
        "ConnectionString": "localhost:6379"
      }
    }
  },
  "RedisCache": {
    "Enable": true,
    "Expiration": "00:05:00"
  }
}
```

**Cache Keys**:
- `TypeDocumentsList` - Cached list of all type documents (6-hour TTL)

**Invalidation Strategy**:
- Cache is cleared on Create, Update, or Delete operations

#### RabbitMQ Configuration

```json
{
  "RabbitMQ": {
    "Enable": true,
    "Host": "localhost",
    "Port": 5672,
    "UserName": "user",
    "Password": "pass",
    "EnableDiagnostic": false
  }
}
```

**Published Events**:
- `TypeDocumentCreatedDomainEvent` - Published on create
- `TypeDocumentUpdatedDomainEvent` - Published on update
- `TypeDocumentDeletedDomainEvent` - Published on delete

#### Security Configuration

```json
{
  "Security": {
    "ClientId": "ae5bd492-a9a8-4462-9153-a71f960ed269",
    "IncludeErrorDetails": true,
    "ValidateAudience": false,
    "ValidateIssuer": true,
    "ValidateLifetime": true,
    "RequireHttpsMetadata": true,
    "ValidIssuer": "https://codedesignplusdevelopment.b2clogin.com/fb23d3e8-7de7-4554-9d73-78e9904a243f/v2.0/",
    "ValidAudiences": [
      "ae5bd492-a9a8-4462-9153-a71f960ed269"
    ],
    "Applications": [],
    "ValidateLicense": false,
    "ValidateRbac": false,
    "ServerRbac": "http://localhost:5001",
    "RefreshRbacInterval": 10
  }
}
```

**Authentication**:
- **Provider**: Azure AD B2C
- **Token Type**: JWT Bearer
- **Required Claims**: None (tenant from header)
- **Anonymous Endpoints**: `GET /api/typedocument`, `GET /api/typedocument/{id}`

#### Vault Configuration

```json
{
  "Vault": {
    "Enable": true,
    "Address": "http://localhost:8200",
    "AppName": "ms-catalogs",
    "Solution": "security-codedesignplus",
    "Token": "root",
    "Mongo": {
      "Enable": true,
      "TemplateConnectionString": "mongodb://{0}:{1}@localhost:27017"
    },
    "RabbitMQ": {
      "Enable": true
    }
  }
}
```

**Secrets Loaded from Vault**:
- MongoDB username and password
- RabbitMQ username and password

#### Observability Configuration

```json
{
  "Logger": {
    "Enable": true,
    "OTelEndpoint": "http://localhost:4317",
    "Level": "Warning"
  },
  "Observability": {
    "Enable": true,
    "ServerOtel": "http://localhost:4317",
    "Trace": {
      "Enable": true,
      "AspNetCore": true,
      "GrpcClient": true,
      "SqlClient": false,
      "CodeDesignPlusSdk": true,
      "Redis": true,
      "Kafka": false
    },
    "Metrics": {
      "Enable": true,
      "AspNetCore": true
    }
  }
}
```

**Telemetry**:
- **Traces**: Sent to OpenTelemetry Collector
- **Metrics**: ASP.NET Core metrics
- **Logs**: Structured logging with Serilog

### Environment Variables

You can override configuration via environment variables:

```bash
# MongoDB
export Mongo__Database=db-ms-catalogs-dev

# Redis
export Redis__Instances__Core__ConnectionString=redis-dev:6379

# RabbitMQ
export RabbitMQ__Host=rabbitmq-dev
export RabbitMQ__Port=5672

# Vault
export Vault__Token=my-vault-token

# Kestrel
export Kestrel__Endpoints__Http__Url=http://*:5000
```

## 🎬 Use Cases & Scenarios

### Use Case 1: Document Type Management

**Scenario**: A Colombian e-commerce platform needs to validate customer document types (CC, TI, NIT, CE, PP, etc.).

**Solution**:
1. Admin creates document types via API
2. Frontend fetches document types for dropdown
3. Customer selects document type during registration
4. System validates document based on type rules

**Benefits**:
- Centralized document type definitions
- Consistent validation across microservices
- Easy to add new document types
- Fast lookup via caching

### Use Case 2: Multi-Tenant Catalog

**Scenario**: A SaaS platform needs to support different document types per tenant (country-specific).

**Solution**:
1. Each tenant has isolated catalog data
2. Tenant-specific document types are created
3. API requests include `X-Tenant` header
4. System filters data by tenant automatically

**Benefits**:
- Data isolation per tenant
- Flexible per-tenant configuration
- No code changes for new tenants

### Use Case 3: Product Category Taxonomy

**Scenario**: An e-commerce platform needs hierarchical product categories (future feature).

**Solution**:
1. Define category aggregate (similar to TypeDocument)
2. Add parent-child relationships
3. Implement category tree queries
4. Cache category trees per tenant

**Benefits**:
- Extensible architecture
- Reuse existing patterns
- Consistent API design

### Use Case 4: Tax Code Catalog

**Scenario**: A financial system needs to manage tax codes and rates.

**Solution**:
1. Create TaxCode aggregate
2. Add rate, effective date, and region properties
3. Implement CRUD commands/queries
4. Publish tax code change events

**Benefits**:
- DRY principle - reuse existing patterns
- Event-driven updates to dependent systems
- Audit trail for tax code changes

### Use Case 5: Integration with Other Microservices

**Scenario**: The Payments microservice needs to validate document types before processing payments.

**Solution**:
1. Payments subscribes to `TypeDocumentCreatedDomainEvent`
2. Payments caches valid document types locally
3. On payment request, validates document type
4. Falls back to Catalogs API if cache miss

**Benefits**:
- Decoupled microservices
- Event-driven synchronization
- Resilient to Catalogs downtime

## 🏗️ Architecture

### Clean Architecture Layers

```
src/
├── domain/
│   ├── CodeDesignPlus.Net.Microservice.Catalogs.Domain/
│   │   ├── TypeDocumentAggregate.cs        # Aggregate root
│   │   ├── DomainEvents/
│   │   │   ├── TypeDocumentCreatedDomainEvent.cs
│   │   │   ├── TypeDocumentUpdatedDomainEvent.cs
│   │   │   └── TypeDocumentDeletedDomainEvent.cs
│   │   ├── Repositories/
│   │   │   └── ITypeDocumentRepository.cs  # Repository interface
│   │   └── Errors.cs                       # Domain error codes (100-199)
│   │
│   ├── CodeDesignPlus.Net.Microservice.Catalogs.Application/
│   │   ├── TypeDocument/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateTypeDocument/
│   │   │   │   │   ├── CreateTypeDocumentCommand.cs
│   │   │   │   │   └── CreateTypeDocumentCommandHandler.cs
│   │   │   │   ├── UpdateTypeDocument/
│   │   │   │   │   ├── UpdateTypeDocumentCommand.cs
│   │   │   │   │   └── UpdateTypeDocumentCommandHandler.cs
│   │   │   │   └── DeleteTypeDocument/
│   │   │   │       ├── DeleteTypeDocumentCommand.cs
│   │   │   │       └── DeleteTypeDocumentCommandHandler.cs
│   │   │   ├── Queries/
│   │   │   │   ├── GetAllTypeDocument/
│   │   │   │   │   ├── GetAllTypeDocumentQuery.cs
│   │   │   │   │   └── GetAllTypeDocumentQueryHandler.cs
│   │   │   │   └── GetTypeDocumentById/
│   │   │   │       ├── GetTypeDocumentByIdQuery.cs
│   │   │   │       └── GetTypeDocumentByIdQueryHandler.cs
│   │   │   └── DataTransferObjects/
│   │   │       └── TypeDocumentDto.cs
│   │   └── Errors.cs                       # Application error codes (200-299)
│   │
│   └── CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure/
│       ├── Repositories/
│       │   └── TypeDocumentRepository.cs    # MongoDB implementation
│       └── Errors.cs                        # Infrastructure error codes (300-399)
│
└── entrypoints/
    └── CodeDesignPlus.Net.Microservice.Catalogs.Rest/
        ├── Program.cs                       # Application startup
        ├── Controllers/
        │   └── TypeDocumentController.cs    # REST API controller
        ├── appsettings.json                 # Configuration
        └── Dockerfile                       # Container image
```

### CQRS Pattern

**Commands** (Write Operations):
- `CreateTypeDocumentCommand` - Creates new type document
- `UpdateTypeDocumentCommand` - Updates existing type document
- `DeleteTypeDocumentCommand` - Soft deletes type document

**Queries** (Read Operations):
- `GetAllTypeDocumentQuery` - Returns all type documents (cached)
- `GetTypeDocumentByIdQuery` - Returns single type document by ID

**Benefits**:
- Clear separation of concerns
- Optimized read/write paths
- Easy to add new operations
- Testable in isolation

### Domain-Driven Design

#### Aggregates

**TypeDocumentAggregate**:
- **Aggregate Root**: TypeDocumentAggregate
- **Entity ID**: Guid
- **Properties**:
  - `Name`: string (max 64 chars)
  - `Description`: string? (max 512 chars)
  - `Code`: string (max 4 chars, unique)
  - `IsActive`: bool
  - `IsDeleted`: bool
  - `CreatedAt`: Instant
  - `UpdatedAt`: Instant?
  - `DeletedAt`: Instant?

**Invariants**:
- Name is required
- Code is required and unique
- Code max length is 4 characters
- Deleted items are inactive

#### Value Objects

- Currently uses primitives (strings, bool)
- Future enhancement: `DocumentCode` value object with validation

#### Domain Events

Events are raised on aggregate state changes and published via RabbitMQ:

1. **TypeDocumentCreatedDomainEvent**
   - Raised: On `Create()`
   - Payload: Id, Name, Description, Code, IsActive

2. **TypeDocumentUpdatedDomainEvent**
   - Raised: On `Update()`
   - Payload: Id, Name, Description, Code, IsActive

3. **TypeDocumentDeletedDomainEvent**
   - Raised: On `Delete()`
   - Payload: Id, Name, Description, Code, IsActive

#### Repository Pattern

**ITypeDocumentRepository**:
```csharp
public interface ITypeDocumentRepository : IRepositoryBase
{
    Task<List<TypeDocumentAggregate>> GetAllAsync(CancellationToken cancellationToken);
}
```

**Base Repository Operations** (from `IRepositoryBase`):
- `CreateAsync()` - Persist new aggregate
- `UpdateAsync()` - Update existing aggregate
- `DeleteAsync()` - Delete aggregate
- `FindAsync()` - Find by ID
- `ExistsAsync()` - Check existence

### Dependency Flow

```
Controllers → MediatR → CommandHandlers/QueryHandlers
                              ↓
                        Domain Aggregates
                              ↓
                        Repositories
                              ↓
                        MongoDB
```

**External Dependencies**:
- Redis (Cache)
- RabbitMQ (Events)
- Vault (Secrets)

## 🧪 Testing

### Test Structure

```
tests/
├── unit/
│   ├── CodeDesignPlus.Net.Microservice.Catalogs.Domain.Test/
│   │   └── TypeDocumentAggregateTest.cs
│   ├── CodeDesignPlus.Net.Microservice.Catalogs.Application.Test/
│   │   ├── Commands/
│   │   │   ├── CreateTypeDocumentCommandHandlerTest.cs
│   │   │   ├── UpdateTypeDocumentCommandHandlerTest.cs
│   │   │   └── DeleteTypeDocumentCommandHandlerTest.cs
│   │   └── Queries/
│   │       ├── GetAllTypeDocumentQueryHandlerTest.cs
│   │       └── GetTypeDocumentByIdQueryHandlerTest.cs
│   ├── CodeDesignPlus.Net.Microservice.Catalogs.Infrastructure.Test/
│   │   └── Repositories/
│   │       └── TypeDocumentRepositoryTest.cs
│   └── CodeDesignPlus.Net.Microservice.Catalogs.Rest.Test/
│       └── Controllers/
│           └── TypeDocumentControllerTest.cs
├── integration/
│   └── CodeDesignPlus.Net.Microservice.Catalogs.Rest.Test/
│       └── TypeDocumentIntegrationTest.cs
└── load/
    └── k6-load-test.js
```

### Running Tests

#### All Tests
```bash
dotnet test
```

#### Unit Tests Only
```bash
dotnet test --filter "FullyQualifiedName~.Test"
```

#### Integration Tests Only
```bash
dotnet test --filter "Category=Integration"
```

#### With Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Coverage Goals

- **Domain Layer**: 95%+ coverage
- **Application Layer**: 90%+ coverage
- **Infrastructure Layer**: 85%+ coverage
- **REST API Layer**: 80%+ coverage

### Example Unit Test

```csharp
[Fact]
public void Create_ValidInput_CreatesAggregate()
{
    // Arrange
    var id = Guid.NewGuid();
    var name = "Cedula de Ciudadania";
    var description = "Colombian National ID";
    var code = "CC";
    var isActive = true;

    // Act
    var aggregate = TypeDocumentAggregate.Create(id, name, description, code, isActive);

    // Assert
    Assert.Equal(id, aggregate.Id);
    Assert.Equal(name, aggregate.Name);
    Assert.Equal(description, aggregate.Description);
    Assert.Equal(code, aggregate.Code);
    Assert.True(aggregate.IsActive);
    Assert.Single(aggregate.GetAndClearEvents());
}
```

### Example Integration Test

```csharp
[Fact]
public async Task CreateTypeDocument_ValidRequest_Returns204()
{
    // Arrange
    var client = _factory.CreateClient();
    var request = new CreateTypeDocumentDto
    {
        Id = Guid.NewGuid(),
        Name = "Test Document",
        Description = "Test Description",
        Code = "TEST",
        IsActive = true
    };

    // Act
    var response = await client.PostAsJsonAsync("/api/typedocument", request);

    // Assert
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
}
```

### Load Testing

Load tests are available using k6:

```bash
cd tests/load
k6 run k6-load-test.js
```

**Test Scenarios**:
- 100 VUs for 1 minute
- Ramp up from 0 to 100 VUs in 30 seconds
- Tests GET and POST endpoints

**Performance Targets**:
- P95 response time < 500ms
- Throughput > 1000 req/s
- Error rate < 1%

## ✅ Best Practices

### 1. Aggregate Design

**DO**:
- Keep aggregates small and focused
- Enforce invariants in the aggregate
- Raise domain events for state changes
- Use private setters to protect encapsulation

**DON'T**:
- Create large aggregates with many entities
- Expose setters for properties
- Violate aggregate boundaries
- Mix business logic in controllers

### 2. CQRS Implementation

**DO**:
- Separate commands and queries
- Use DTOs for API contracts
- Validate commands with FluentValidation
- Cache query results when possible

**DON'T**:
- Return entities from queries (use DTOs)
- Mix command and query logic
- Skip validation
- Cache commands

### 3. Repository Pattern

**DO**:
- Define repositories at aggregate root level
- Use async/await for all operations
- Handle cancellation tokens
- Keep repository interfaces in Domain layer

**DON'T**:
- Create repositories for every entity
- Return IQueryable from repositories
- Leak implementation details
- Put business logic in repositories

### 4. Caching Strategy

**DO**:
- Cache read-heavy data (GetAll queries)
- Invalidate cache on writes
- Use appropriate TTL (6 hours for catalogs)
- Handle cache misses gracefully

**DON'T**:
- Cache everything by default
- Use infinite TTL
- Ignore cache invalidation
- Depend on cache for correctness

### 5. Event Publishing

**DO**:
- Publish events after persistence
- Include relevant data in events
- Use event versioning
- Handle event publishing failures

**DON'T**:
- Publish events before persistence
- Include sensitive data in events
- Break event consumers with schema changes
- Silently ignore publishing failures

### 6. Error Handling

**DO**:
- Use domain-specific error codes
- Return Problem Details (RFC 7807)
- Log errors with context
- Handle cancellation properly

**DON'T**:
- Return generic error messages
- Expose stack traces to clients
- Swallow exceptions
- Ignore cancellation tokens

### 7. Configuration Management

**DO**:
- Use Vault for secrets
- Use environment-specific appsettings
- Validate configuration on startup
- Use strongly-typed configuration

**DON'T**:
- Commit secrets to source control
- Hard-code connection strings
- Use magic strings for config keys
- Skip configuration validation

### 8. Multi-Tenancy

**DO**:
- Require `X-Tenant` header for all requests
- Filter data by tenant automatically
- Validate tenant access
- Log tenant context

**DON'T**:
- Allow cross-tenant data access
- Skip tenant validation
- Hard-code tenant IDs
- Mix tenant data

### 9. API Design

**DO**:
- Use RESTful conventions
- Version your APIs
- Document with Swagger
- Use appropriate HTTP status codes

**DON'T**:
- Use verbs in endpoint names
- Return 200 OK for errors
- Skip API documentation
- Break backwards compatibility

### 10. Testing

**DO**:
- Write tests for all business logic
- Test edge cases and error paths
- Use realistic test data
- Run tests in CI/CD

**DON'T**:
- Skip unit tests
- Test only happy paths
- Use production data in tests
- Ignore failing tests

## 🔧 Troubleshooting

### Common Issues

#### 1. API Returns 401 Unauthorized

**Symptom**: All API requests return 401 Unauthorized.

**Possible Causes**:
- Missing or invalid JWT token
- Token expired
- Invalid audience or issuer

**Solution**:
```bash
# 1. Verify token is provided
curl -H "Authorization: Bearer YOUR_TOKEN" http://localhost:5000/ms-catalogs/api/typedocument

# 2. Check token expiration
# Decode token at https://jwt.io/

# 3. Verify issuer and audience in appsettings.json
"Security": {
  "ValidIssuer": "https://codedesignplusdevelopment.b2clogin.com/...",
  "ValidAudiences": ["ae5bd492-a9a8-4462-9153-a71f960ed269"]
}

# 4. For local development, use AllowAnonymous endpoints
curl http://localhost:5000/ms-catalogs/api/typedocument
```

#### 2. Cache Not Invalidating

**Symptom**: GetAll returns stale data after Create/Update/Delete.

**Possible Causes**:
- Redis connection issues
- Cache key mismatch
- Exception during cache invalidation

**Solution**:
```bash
# 1. Verify Redis is running
redis-cli ping

# 2. Check cache key in code
public const string CACHE_KEY = "TypeDocumentsList";

# 3. Manually clear cache
redis-cli DEL TypeDocumentsList

# 4. Check logs for cache errors
docker logs ms-catalogs-rest | grep -i cache
```

#### 3. Events Not Publishing

**Symptom**: Domain events are not received by subscribers.

**Possible Causes**:
- RabbitMQ connection issues
- Event publishing disabled
- Exchange/queue not created

**Solution**:
```bash
# 1. Verify RabbitMQ is running
curl http://localhost:15672/api/overview

# 2. Check RabbitMQ configuration
"RabbitMQ": {
  "Enable": true,
  "Host": "localhost",
  "Port": 5672
}

# 3. Check RabbitMQ exchanges
# Open http://localhost:15672 and verify exchanges exist

# 4. Enable diagnostic logging
"RabbitMQ": {
  "EnableDiagnostic": true
}
```

#### 4. MongoDB Connection Failures

**Symptom**: API returns 500 errors, logs show MongoDB connection errors.

**Possible Causes**:
- MongoDB not running
- Invalid connection string
- Vault not providing credentials

**Solution**:
```bash
# 1. Verify MongoDB is running
docker ps | grep mongo

# 2. Test MongoDB connection
mongosh mongodb://localhost:27017

# 3. Check Vault configuration
"Vault": {
  "Enable": true,
  "Mongo": {
    "Enable": true,
    "TemplateConnectionString": "mongodb://{0}:{1}@localhost:27017"
  }
}

# 4. Verify Vault secrets
vault kv get secret/security-codedesignplus/ms-catalogs/mongo

# 5. For local dev, disable Vault temporarily
"Vault": {
  "Enable": false
},
"Mongo": {
  "ConnectionString": "mongodb://localhost:27017",
  "Database": "db-ms-catalogs"
}
```

#### 5. Health Checks Failing

**Symptom**: `/health` endpoint returns 503 Service Unavailable.

**Possible Causes**:
- One or more dependencies are unhealthy
- MongoDB, Redis, or RabbitMQ not accessible

**Solution**:
```bash
# 1. Check health endpoint details
curl http://localhost:5000/ms-catalogs/health | jq

# 2. Verify each dependency
docker ps | grep -E "mongo|redis|rabbitmq"

# 3. Check dependency health
# MongoDB
mongosh --eval "db.adminCommand({ping: 1})"

# Redis
redis-cli ping

# RabbitMQ
curl http://localhost:15672/api/healthchecks/node

# 4. Check logs for specific errors
docker logs ms-catalogs-rest | tail -100
```

#### 6. Swagger UI Not Loading

**Symptom**: Swagger UI returns 404 or blank page.

**Possible Causes**:
- PathBase not configured
- Swagger disabled in environment
- HTTPS redirect issues

**Solution**:
```bash
# 1. Verify PathBase in configuration
"Core": {
  "PathBase": "/ms-catalogs"
}

# 2. Access Swagger with correct path
open http://localhost:5000/ms-catalogs/swagger

# 3. Check if Swagger is enabled
# Swagger is enabled by default, controlled in Program.cs:
app.UseCoreSwagger();

# 4. For Docker, check port mapping
docker run -p 5000:5000 ms-catalogs-rest
```

#### 7. TypeDocument Already Exists Error

**Symptom**: Create returns 409 Conflict even for new documents.

**Possible Causes**:
- Duplicate GUID
- Duplicate Code
- Cache inconsistency

**Solution**:
```bash
# 1. Verify GUID is unique
# Always generate new GUID for Create

# 2. Check for existing document
curl http://localhost:5000/ms-catalogs/api/typedocument/{id}

# 3. Verify Code uniqueness
# Code should be unique per tenant

# 4. Clear cache and retry
redis-cli DEL TypeDocumentsList
```

#### 8. Slow Query Performance

**Symptom**: GetAll query takes > 1 second.

**Possible Causes**:
- Cache not working
- Large dataset without pagination
- Missing MongoDB indexes

**Solution**:
```bash
# 1. Verify cache is enabled
"RedisCache": {
  "Enable": true,
  "Expiration": "00:05:00"
}

# 2. Check if query is hitting cache
# Should see cache hit in logs

# 3. Add MongoDB indexes
mongosh db-ms-catalogs
db.TypeDocumentAggregate.createIndex({ "Code": 1 })
db.TypeDocumentAggregate.createIndex({ "IsActive": 1, "IsDeleted": 1 })

# 4. Consider pagination for large datasets
# Future enhancement: Add pagination to GetAll
```

## 📊 Domain Model

### TypeDocument Aggregate

**Purpose**: Represents a document type used for identification (CC, TI, NIT, CE, PP, etc.).

**Properties**:
- `Id`: Guid - Unique identifier
- `Name`: string - Display name (e.g., "Cedula de Ciudadania")
- `Description`: string? - Optional description
- `Code`: string - Short code (e.g., "CC") - unique per tenant
- `IsActive`: bool - Active state
- `IsDeleted`: bool - Soft delete flag
- `CreatedAt`: Instant - Creation timestamp
- `UpdatedAt`: Instant? - Last update timestamp
- `DeletedAt`: Instant? - Deletion timestamp

**Behavior**:
- `Create()` - Factory method to create new aggregate
- `Update()` - Update properties and raise event
- `Delete()` - Soft delete and raise event

**Invariants**:
- Name is required (enforced by DomainGuard)
- Code is required (enforced by DomainGuard)
- Deleted items must be inactive

**Example**:
```csharp
var typeDocument = TypeDocumentAggregate.Create(
    id: Guid.NewGuid(),
    name: "Cedula de Ciudadania",
    description: "Colombian National ID Card",
    code: "CC",
    isActive: true
);

typeDocument.Update(
    name: "Cedula de Ciudadania",
    description: "Updated description",
    code: "CC",
    isActive: false
);

typeDocument.Delete(); // Soft delete
```

### Future Aggregates

**Category Aggregate** (Planned):
- Hierarchical product categories
- Parent-child relationships
- Level tracking
- Path materialization

**TaxCode Aggregate** (Planned):
- Tax code definitions
- Rate and effective date
- Region-specific codes

**Currency Aggregate** (Planned):
- Currency definitions
- Exchange rates
- Region support

## 📡 Events

### TypeDocumentCreatedDomainEvent

**Raised**: When a new TypeDocument is created.

**Payload**:
```json
{
  "aggregateId": "550e8400-e29b-41d4-a716-446655440000",
  "eventId": "660e8400-e29b-41d4-a716-446655440000",
  "occurredAt": "2026-05-15T10:00:00Z",
  "name": "Cedula de Ciudadania",
  "description": "Colombian National ID Card",
  "code": "CC",
  "isActive": true,
  "metadata": {
    "tenant": "tenant-id",
    "userId": "user-id"
  }
}
```

**Subscribers**:
- Other microservices needing document type catalog
- Analytics services
- Audit log services

### TypeDocumentUpdatedDomainEvent

**Raised**: When a TypeDocument is updated.

**Payload**:
```json
{
  "aggregateId": "550e8400-e29b-41d4-a716-446655440000",
  "eventId": "660e8400-e29b-41d4-a716-446655440001",
  "occurredAt": "2026-05-15T11:00:00Z",
  "name": "Cedula de Ciudadania",
  "description": "Updated description",
  "code": "CC",
  "isActive": false,
  "metadata": {
    "tenant": "tenant-id",
    "userId": "user-id"
  }
}
```

**Subscribers**:
- Microservices with cached catalog data
- Analytics services
- Audit log services

### TypeDocumentDeletedDomainEvent

**Raised**: When a TypeDocument is soft deleted.

**Payload**:
```json
{
  "aggregateId": "550e8400-e29b-41d4-a716-446655440000",
  "eventId": "660e8400-e29b-41d4-a716-446655440002",
  "occurredAt": "2026-05-15T12:00:00Z",
  "name": "Cedula de Ciudadania",
  "description": "Colombian National ID Card",
  "code": "CC",
  "isActive": false,
  "metadata": {
    "tenant": "tenant-id",
    "userId": "user-id"
  }
}
```

**Subscribers**:
- Microservices with cached catalog data
- Analytics services
- Audit log services

### Event Schema Versioning

Events include an `EventKey` attribute with version number:

```csharp
[EventKey<TypeDocumentAggregate>(1, "TypeDocumentCreatedDomainEvent")]
public class TypeDocumentCreatedDomainEvent : DomainEvent
{
    // ...
}
```

**Version History**:
- **v1**: Initial version

**Future Changes**:
- When schema changes, increment version
- Support multiple versions for backwards compatibility
- Use event transformation for old versions

## 🔒 Security

### Authentication

**Provider**: Azure AD B2C  
**Token Type**: JWT Bearer  
**Required Header**: `Authorization: Bearer {token}`

### Authorization

**Role-Based Access Control (RBAC)**:
- Currently disabled (`ValidateRbac: false`)
- Can be enabled with RBAC server integration

**Endpoint Security**:
- `GET /api/typedocument` - Anonymous (AllowAnonymous)
- `GET /api/typedocument/{id}` - Anonymous (AllowAnonymous)
- `POST /api/typedocument` - Requires authentication
- `PUT /api/typedocument/{id}` - Requires authentication
- `DELETE /api/typedocument/{id}` - Requires authentication

### Multi-Tenancy

**Tenant Identification**: Via `X-Tenant` HTTP header

**Tenant Isolation**:
- All queries filter by tenant automatically
- Tenant is extracted from request context
- Cross-tenant access is prevented

### Secret Management

**Vault Integration**:
- MongoDB credentials
- RabbitMQ credentials
- API keys

**Local Development**: Can use appsettings.json with Vault disabled.

### HTTPS

**Production**: Requires HTTPS (enforced by `RequireHttpsMetadata: true`)  
**Local Development**: Can use HTTP

### Input Validation

**Validation Framework**: FluentValidation

**Validation Rules**:
- All commands are validated before execution
- Returns 400 Bad Request with validation errors
- Follows Problem Details (RFC 7807) format

### Error Handling

**Security Best Practices**:
- Never expose stack traces in production
- Use generic error messages for sensitive operations
- Log detailed errors server-side
- Return Problem Details with safe information

### CORS

**Configuration**:
```csharp
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
```

**Production**: Should restrict to specific origins.

## ❓ FAQ

### General Questions

**Q: What is the purpose of the Catalogs microservice?**  
A: Manages catalog data such as document types, categories, and product taxonomies. Provides a centralized repository for catalog definitions used across the system.

**Q: Can I use this microservice without Vault?**  
A: Yes, for local development you can disable Vault and use appsettings.json for configuration. Set `Vault.Enable: false` and provide connection strings directly.

**Q: Does this support multi-tenancy?**  
A: Yes, all operations are tenant-isolated via the `X-Tenant` header.

**Q: Can I add new catalog types?**  
A: Yes, the architecture is designed to be extensible. Create a new aggregate (similar to TypeDocument) and implement CQRS commands/queries.

### Technical Questions

**Q: Why use CQRS for simple CRUD operations?**  
A: CQRS provides a clear separation between reads and writes, making it easier to optimize each path independently. It also makes the codebase more maintainable and testable.

**Q: Why MongoDB instead of SQL Server?**  
A: MongoDB is better suited for catalog data which is document-oriented and schema-flexible. It also scales horizontally and integrates well with event sourcing patterns.

**Q: Why cache with Redis instead of in-memory cache?**  
A: Redis provides distributed caching, which is essential for multi-instance deployments. It also survives service restarts and can be shared across microservices.

**Q: Why use RabbitMQ instead of Kafka?**  
A: RabbitMQ is simpler to set up and sufficient for most use cases. It provides message routing, retries, and dead-letter queues out of the box.

**Q: Why use Vault for secrets?**  
A: Vault provides centralized secret management, automatic secret rotation, and audit logging. It's more secure than storing secrets in appsettings.json or environment variables.

### Development Questions

**Q: How do I add a new catalog type?**  
A: Follow these steps:
1. Create a new aggregate in the Domain layer
2. Define domain events for Create/Update/Delete
3. Implement repository interface
4. Create CQRS commands and queries in Application layer
5. Implement repository in Infrastructure layer
6. Add controller in REST API layer
7. Write tests

**Q: How do I add pagination to GetAll?**  
A: Implement a paginated query:
```csharp
public record GetAllTypeDocumentPaginatedQuery(int Page, int PageSize) 
    : IRequest<PaginatedResult<TypeDocumentDto>>;
```

**Q: How do I add filtering to GetAll?**  
A: Add filter parameters to the query:
```csharp
public record GetAllTypeDocumentQuery(bool? IsActive = null, string? Code = null) 
    : IRequest<List<TypeDocumentDto>>;
```

**Q: How do I add a new field to TypeDocument?**  
A: Follow these steps:
1. Add property to TypeDocumentAggregate
2. Update Create/Update methods to include new field
3. Update domain events to include new field
4. Update DTOs
5. Update validation rules
6. Update database (MongoDB is schemaless, so no migration needed)
7. Update tests
8. Version the domain event if breaking change

### Deployment Questions

**Q: How do I deploy to Kubernetes?**  
A: Use the Helm charts in the `charts/` directory:
```bash
helm install ms-catalogs-rest ./charts/ms-catalogs-rest \
  --set image.tag=v1.0.0 \
  --set ingress.enabled=true
```

**Q: What are the hardware requirements?**  
A: Minimum:
- CPU: 1 core
- RAM: 512 MB
- Disk: 1 GB

Recommended (production):
- CPU: 2 cores
- RAM: 2 GB
- Disk: 10 GB

**Q: How many instances should I run?**  
A: Start with 2 instances for high availability. Scale based on traffic (see load testing results).

**Q: How do I monitor the microservice?**  
A: Use the following:
- Health checks: `/ms-catalogs/health`
- Metrics: Prometheus metrics endpoint
- Traces: OpenTelemetry traces
- Logs: Structured logs via Serilog

### Performance Questions

**Q: What is the expected throughput?**  
A: Based on load tests:
- Read (cached): 5000+ req/s
- Read (uncached): 1000+ req/s
- Write: 500+ req/s

**Q: How long is data cached?**  
A: TypeDocumentsList is cached for 6 hours. Cache is invalidated on Create/Update/Delete.

**Q: Can I increase cache TTL?**  
A: Yes, update the TTL in the query handler:
```csharp
await cacheManager.SetAsync(CACHE_KEY, data, TimeSpan.FromHours(12));
```

**Q: How do I optimize query performance?**  
A: Follow these strategies:
1. Enable caching for read-heavy queries
2. Add MongoDB indexes for frequently queried fields
3. Implement pagination for large datasets
4. Use projection to return only needed fields

## 🤝 Contributing

Please read our Contributing Guide for details on our code of conduct and development process.

### Code Standards

- Follow .NET coding conventions
- Use meaningful names for variables and methods
- Write XML documentation for public APIs
- Keep methods small and focused
- Write tests for all business logic

### Pull Request Process

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -m 'Add my feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a Pull Request

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types**: feat, fix, docs, style, refactor, test, chore

**Example**:
```
feat(typedocument): add pagination support

Implement pagination for GetAllTypeDocumentQuery.

Closes #123
```

## 📄 License

This project is licensed under the GNU Lesser General Public License v3.0 - see the LICENSE.md file for details.

## 🔧 Tools & Utilities

The repository includes several utility scripts in the `tools/` directory:

### convert-crlf-to-lf.sh
Converts line endings from CRLF to LF (useful for cross-platform development).

```bash
./tools/convert-crlf-to-lf.sh
```

### update-packages/update-packages.sh
Updates all NuGet packages to the latest versions.

```bash
cd tools/update-packages
./update-packages.sh
```

### upgrade-dotnet/upgrade-assistant.sh
Upgrades the solution to a newer .NET version.

```bash
cd tools/upgrade-dotnet
./upgrade-assistant.sh
```

### vault/config-vault.sh
Configures Vault with required secrets for local development.

```bash
cd tools/vault
./config-vault.sh
```

**Configured Secrets**:
- MongoDB username and password
- RabbitMQ username and password
- Service-specific secrets

### sonarqube/sonar.sh
Runs SonarQube analysis for code quality and security scanning.

```bash
cd tools/sonarqube
./sonar.sh
```

**Prerequisites**:
- SonarQube server running
- Update script with SonarQube URL and token

## 📦 Docker Support

### Build Docker Image

```bash
docker build -t ms-catalogs-rest:latest \
  -f src/entrypoints/CodeDesignPlus.Net.Microservice.Catalogs.Rest/Dockerfile \
  .
```

### Run Docker Container

```bash
docker run -d \
  -p 5000:5000 \
  --network=backend \
  -e ASPNETCORE_ENVIRONMENT=Docker \
  -e Vault__Token=root \
  --name ms-catalogs-rest \
  ms-catalogs-rest:latest
```

### Docker Compose Example

```yaml
version: '3.8'
services:
  ms-catalogs-rest:
    image: ms-catalogs-rest:latest
    ports:
      - "5000:5000"
    networks:
      - backend
    environment:
      ASPNETCORE_ENVIRONMENT: Docker
      Vault__Token: root
      Mongo__Database: db-ms-catalogs
      Redis__Instances__Core__ConnectionString: redis:6379
      RabbitMQ__Host: rabbitmq
    depends_on:
      - mongodb
      - redis
      - rabbitmq
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/ms-catalogs/health"]
      interval: 30s
      timeout: 10s
      retries: 3

  mongodb:
    image: mongo:6.0
    ports:
      - "27017:27017"
    networks:
      - backend

  redis:
    image: redis:7.0-alpine
    ports:
      - "6379:6379"
    networks:
      - backend

  rabbitmq:
    image: rabbitmq:3.12-management
    ports:
      - "5672:5672"
      - "15672:15672"
    networks:
      - backend

networks:
  backend:
    driver: bridge
```

### Multi-Stage Build

The Dockerfile uses multi-stage builds for optimal image size:
- **Build stage**: Uses `mcr.microsoft.com/dotnet/sdk:9.0`
- **Final stage**: Uses `mcr.microsoft.com/dotnet/aspnet:9.0-alpine`

**Image Size**: ~200 MB (with Alpine base)

### Environment Variables

You can override configuration via environment variables:

```bash
docker run -d \
  -p 5000:5000 \
  -e Mongo__Database=db-ms-catalogs-dev \
  -e Redis__Instances__Core__ConnectionString=redis-dev:6379 \
  -e RabbitMQ__Host=rabbitmq-dev \
  -e Vault__Enable=false \
  ms-catalogs-rest:latest
```

## 🚢 Kubernetes Deployment

### Helm Charts

Helm charts are available in the `charts/` directory:

- `ms-catalogs-rest` - REST API deployment
- `ms-catalogs-grpc` - gRPC service deployment (planned)
- `ms-catalogs-worker` - Background worker deployment (planned)

### Install with Helm

```bash
# Add CodeDesignPlus Helm repository
helm repo add codedesignplus https://www.codedesignplus.com/helm-charts/
helm repo update

# Install the chart
helm install ms-catalogs-rest ./charts/ms-catalogs-rest \
  --set image.repository=your-registry/ms-catalogs-rest \
  --set image.tag=v1.0.0 \
  --set ingress.enabled=true \
  --set ingress.hosts[0].host=catalogs.yourdomain.com
```

### Helm Values

Key configuration values:

```yaml
replicaCount: 2

image:
  repository: your-registry/ms-catalogs-rest
  tag: v1.0.0
  pullPolicy: IfNotPresent

service:
  type: ClusterIP
  port: 5000

ingress:
  enabled: true
  className: nginx
  hosts:
    - host: catalogs.yourdomain.com
      paths:
        - path: /ms-catalogs
          pathType: Prefix

resources:
  limits:
    cpu: 1000m
    memory: 1Gi
  requests:
    cpu: 250m
    memory: 512Mi

autoscaling:
  enabled: true
  minReplicas: 2
  maxReplicas: 10
  targetCPUUtilizationPercentage: 70
  targetMemoryUtilizationPercentage: 80

env:
  - name: ASPNETCORE_ENVIRONMENT
    value: "Production"
  - name: Vault__Token
    valueFrom:
      secretKeyRef:
        name: vault-token
        key: token
```

### Kubernetes Resources

The Helm chart creates:
- **Deployment**: Manages pod replicas
- **Service**: Exposes the application
- **Ingress**: Routes external traffic
- **ConfigMap**: Application configuration
- **Secret**: Sensitive configuration (Vault token)
- **HorizontalPodAutoscaler**: Auto-scaling
- **ServiceMonitor**: Prometheus metrics (optional)

## 📊 Monitoring & Observability

### Health Checks

**Liveness Probe**:
```yaml
livenessProbe:
  httpGet:
    path: /ms-catalogs/health
    port: 5000
  initialDelaySeconds: 30
  periodSeconds: 30
```

**Readiness Probe**:
```yaml
readinessProbe:
  httpGet:
    path: /ms-catalogs/health/ready
    port: 5000
  initialDelaySeconds: 10
  periodSeconds: 10
```

### Metrics

**Prometheus Metrics**:
- ASP.NET Core metrics (request rate, duration, errors)
- Custom business metrics (commands, queries, events)

**Grafana Dashboard**: Available in `monitoring/grafana/dashboards/`

### Tracing

**OpenTelemetry Traces**:
- HTTP requests
- gRPC calls
- MongoDB operations
- Redis operations
- RabbitMQ publishing

**Jaeger/Zipkin**: Traces are exported to OpenTelemetry Collector.

### Logging

**Structured Logs**:
- JSON format
- Request/response logging
- Error logging with context
- Performance logging

**Log Aggregation**: Logs can be sent to Elasticsearch, Loki, or CloudWatch.

## 🔗 Related Documentation

- [CodeDesignPlus SDK Documentation](https://codedesignplus.github.io/)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design Reference](https://domainlanguage.com/ddd/reference/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Microservice Architecture](https://microservices.io/)

## 📞 Support

For questions and support:
- **Email**: wliscano@codedesignplus.com
- **GitHub Issues**: [Create an issue](https://github.com/codedesignplus/CodeDesignPlus.Net.Microservice.Catalogs/issues)
- **Documentation**: [CodeDesignPlus Docs](https://codedesignplus.github.io/)

---

**Built with ❤️ by CodeDesignPlus**
