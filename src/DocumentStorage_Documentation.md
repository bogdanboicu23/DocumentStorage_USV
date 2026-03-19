# DocumentStorage System - Technical Documentation

## Executive Summary

DocumentStorage is an enterprise-grade cloud document management system built on the Microsoft .NET 8.0 platform. The system provides secure file storage, user authentication, subscription-based access control, and real-time usage monitoring through a modern web interface.

## Table of Contents

1. [System Overview](#system-overview)
2. [Architecture Design](#architecture-design)
3. [Technology Stack](#technology-stack)
4. [Project Structure](#project-structure)
5. [Core Features](#core-features)
6. [Authentication & Security](#authentication--security)
7. [Database Design](#database-design)
8. [API Documentation](#api-documentation)
9. [User Interface](#user-interface)
10. [Performance & Caching](#performance--caching)
11. [Deployment Guide](#deployment-guide)
12. [Testing Strategy](#testing-strategy)
13. [Troubleshooting](#troubleshooting)
14. [Future Roadmap](#future-roadmap)

---

## 1. System Overview

### Purpose
DocumentStorage provides organizations and individuals with a centralized platform for document management, offering secure storage, organization, and retrieval capabilities with tiered subscription models.

### Key Objectives
- **Security First**: Implement robust authentication and authorization mechanisms
- **Scalability**: Support growing user bases and document volumes
- **Performance**: Ensure fast document access through intelligent caching
- **User Experience**: Provide intuitive interfaces for document management
- **Flexibility**: Offer multiple subscription tiers to meet diverse needs

### Target Users
- **Individual Users**: Personal document storage and organization
- **Small Businesses**: Team collaboration and document sharing
- **Enterprises**: Large-scale document management with advanced features
- **Developers**: API access for custom integrations

---

## 2. Architecture Design

### Clean Architecture Pattern

The system implements Clean Architecture principles with clear separation of concerns across multiple layers:

#### Presentation Layer
- **Blazor Client**: Interactive web UI for end users
- **MVC Admin Panel**: Administrative interface for system management
- **API Controllers**: RESTful endpoints for client communication

#### Business Layer
- **Services**: Core business logic implementation
- **Interfaces**: Contract definitions for dependency injection
- **Validators**: Business rule enforcement

#### Data Access Layer
- **Repositories**: Data access abstraction
- **Entity Framework Core**: ORM for database operations
- **Database Context**: Centralized database configuration

#### Infrastructure Layer
- **Authentication**: JWT token generation and validation
- **Caching**: Memory cache implementation
- **File Storage**: Physical file management

### Design Patterns

#### Repository Pattern
```csharp
public interface IDocumentRepository
{
    Task<Document> GetByIdAsync(Guid id);
    Task<IEnumerable<Document>> GetAllAsync();
    Task AddAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(Guid id);
}
```

#### Service Pattern
```csharp
public interface IDocumentService
{
    Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto dto);
    Task<DocumentDto> GetDocumentByIdAsync(Guid id);
    Task<IEnumerable<DocumentDto>> GetAllDocumentsAsync();
    Task DeleteDocumentAsync(Guid id);
}
```

#### Dependency Injection
All services and repositories are registered in the IoC container for loose coupling and testability.

### Data Flow

1. **Client Request** → HTTP request to API endpoint
2. **Controller** → Receives request, performs validation
3. **Service Layer** → Processes business logic
4. **Repository Layer** → Executes database operations
5. **Database** → Stores/retrieves data
6. **Response Flow** → Returns through layers to client

---

## 3. Technology Stack

### Backend Technologies

#### Core Framework
- **ASP.NET Core 8.0**: Latest LTS framework
- **C# 12**: Modern language features
- **.NET Runtime 8.0**: Cross-platform execution

#### Data Access
- **Entity Framework Core 8.0**: Code-first ORM
- **SQL Server 2019+**: Relational database
- **LINQ**: Query composition

#### Security
- **JWT Bearer Tokens**: Stateless authentication
- **BCrypt.Net**: Password hashing
- **CORS**: Cross-origin resource sharing

#### Caching
- **IMemoryCache**: In-memory caching
- **Distributed Cache Ready**: Redis support capability

### Frontend Technologies

#### Blazor Server
- **Component-Based Architecture**: Reusable UI components
- **Real-time Updates**: SignalR connection
- **Server-Side Rendering**: SEO-friendly

#### Styling
- **Tailwind CSS**: Utility-first CSS framework
- **Custom Components**: Branded UI elements
- **Responsive Design**: Mobile-first approach

#### JavaScript Interop
- **File Downloads**: Browser API integration
- **Notifications**: Toast messages

### Development Tools

#### IDEs
- **Visual Studio 2022**: Full-featured IDE
- **Visual Studio Code**: Lightweight editor
- **JetBrains Rider**: Cross-platform IDE

#### Version Control
- **Git**: Source control
- **GitHub/Azure DevOps**: Repository hosting

#### Package Management
- **NuGet**: .NET package manager
- **npm**: Frontend dependencies

---

## 4. Project Structure

### Solution Organization

```
DocumentStorage_USV/
│
├── src/
│   ├── DocumentStorage.API/              # Web API Project
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── DocumentsController.cs
│   │   │   └── PlansController.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Document/
│   │   │   └── Plan/
│   │   ├── Services/
│   │   │   └── TokenService.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── DocumentStorage.BusinessLayer/    # Business Logic
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── DocumentService.cs
│   │   │   ├── PlanService.cs
│   │   │   └── UsageService.cs
│   │   └── Interfaces/
│   │       └── IServiceInterfaces.cs
│   │
│   ├── DocumentStorage.Client/           # Blazor Client
│   │   ├── Components/
│   │   │   ├── Documents/
│   │   │   │   ├── DocumentList.razor
│   │   │   │   ├── FileUpload.razor
│   │   │   │   └── PaywallModal.razor
│   │   │   ├── Layout/
│   │   │   └── Pages/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Program.cs
│   │
│   ├── DocumentStorage.Data/            # Data Access Layer
│   │   ├── Models/
│   │   │   ├── Document.cs
│   │   │   ├── User.cs
│   │   │   ├── Account.cs
│   │   │   └── Plan.cs
│   │   ├── Repositories/
│   │   ├── Interfaces/
│   │   └── DocumentStorageDbContext.cs
│   │
│   ├── DocumentStorage.MVC/             # Admin Panel
│   │   ├── Controllers/
│   │   ├── Views/
│   │   └── Models/
│   │
│   └── DocumentStorage.Shared/          # Shared DTOs
│       └── DTOs/
│
├── tests/
│   ├── DocumentStorage.UnitTests/
│   └── DocumentStorage.IntegrationTests/
│
├── docs/
│   ├── README.md
│   └── API_Documentation.md
│
└── DocumentStorage_USV.sln
```

### Key Directories

#### API Layer (`DocumentStorage.API`)
- **Controllers**: HTTP endpoint definitions
- **DTOs**: Data transfer objects
- **Services**: API-specific services
- **Middleware**: Request/response pipeline

#### Business Layer (`DocumentStorage.BusinessLayer`)
- **Services**: Core business logic
- **Validators**: Business rule validation
- **Mappers**: Entity to DTO mapping
- **Extensions**: Helper methods

#### Data Layer (`DocumentStorage.Data`)
- **Models**: Entity definitions
- **Repositories**: Data access implementation
- **Migrations**: Database schema changes
- **Configurations**: Entity configurations

#### Client Layer (`DocumentStorage.Client`)
- **Components**: Blazor components
- **Services**: Client-side services
- **Models**: Client-side models
- **wwwroot**: Static assets

---

## 5. Core Features

### Document Management

#### Upload Functionality
```csharp
[HttpPost("upload")]
[Consumes("multipart/form-data")]
public async Task<ActionResult<DocumentDto>> UploadDocument(IFormFile file)
{
    // 1. Validate file
    // 2. Check user limits
    // 3. Save to storage
    // 4. Create database record
    // 5. Return document metadata
}
```

**Features**:
- Multipart form data handling
- File size validation
- Content type detection
- Storage limit enforcement
- Progress tracking

#### Download Functionality
```csharp
[HttpGet("{id}/download")]
public async Task<ActionResult> DownloadDocument(Guid id)
{
    // 1. Retrieve document metadata
    // 2. Check permissions
    // 3. Get file from cache or storage
    // 4. Return file stream
}
```

**Features**:
- Secure file access
- Cache integration
- Content type headers
- Range request support

#### Document Operations
- **Create**: Upload new documents
- **Read**: View document metadata
- **Update**: Modify document properties
- **Delete**: Soft/hard delete options
- **Search**: Find documents by criteria
- **Share**: Generate sharing links

### User Authentication

#### Registration Process
1. User provides email and password
2. Password validation (strength requirements)
3. Password hashing with BCrypt
4. Account creation
5. Welcome email (if configured)

#### Login Process
1. Email/password submission
2. Credential validation
3. JWT token generation
4. Token return to client
5. Client stores token

#### Token Management
```csharp
public class TokenService
{
    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Subscription Management

#### Plan Tiers

| Feature | Free | Basic | Professional | Enterprise |
|---------|------|-------|--------------|------------|
| **Storage** | 1 GB | 5 GB | 50 GB | 500 GB |
| **Documents** | 100 | 500 | Unlimited | Unlimited |
| **Users** | 1 | 5 | 25 | Unlimited |
| **Support** | Community | Email | Priority | 24/7 |
| **Features** | Basic | Standard | Advanced | Custom |
| **Price** | $0 | $9.99/mo | $29.99/mo | $99.99/mo |

#### Usage Tracking
```csharp
public class UsageService : IUsageService
{
    public async Task<UsageDto> GetCurrentUsageAsync(Guid accountId)
    {
        var storageUsed = await _documentRepository.GetAccountStorageUsageAsync(accountId);
        var documentCount = await _documentRepository.GetAccountDocumentCountAsync(accountId);
        var subscription = await _subscriptionRepository.GetActiveSubscriptionAsync(accountId);

        return new UsageDto
        {
            StorageUsedBytes = storageUsed,
            DocumentCount = documentCount,
            StorageLimit = subscription?.Plan.StorageLimit ?? FREE_STORAGE_LIMIT,
            DocumentLimit = subscription?.Plan.DocumentLimit ?? FREE_DOCUMENT_LIMIT
        };
    }
}
```

#### Paywall Implementation
- Real-time limit checking
- Graceful degradation
- Upgrade prompts
- Seamless upgrade flow
- Automatic retry after upgrade

---

## 6. Authentication & Security

### JWT Implementation

#### Token Structure
```json
{
  "header": {
    "alg": "HS256",
    "typ": "JWT"
  },
  "payload": {
    "sub": "user-guid",
    "email": "user@example.com",
    "role": "Owner",
    "exp": 1234567890,
    "iat": 1234567890
  },
  "signature": "..."
}
```

#### Authorization Middleware
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
```

### Password Security

#### BCrypt Configuration
- **Salt Rounds**: 12 (configurable)
- **Hash Algorithm**: BCrypt 2b
- **Password Requirements**:
  - Minimum 8 characters
  - At least one uppercase
  - At least one lowercase
  - At least one number
  - At least one special character

#### Password Storage
```csharp
public class AuthService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### Security Best Practices

#### Input Validation
- SQL injection prevention via parameterized queries
- XSS protection through input sanitization
- CSRF token validation
- File upload restrictions

#### Access Control
- Role-based permissions (Owner, Member, Admin)
- Resource-based authorization
- API rate limiting
- IP whitelist support

#### Data Protection
- HTTPS enforcement
- Encryption at rest (database)
- Encryption in transit (TLS 1.2+)
- Secure file storage with GUID prefixes

---

## 7. Database Design

### Entity Relationships

#### Core Entities

```sql
-- Users Table
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    CreatedAt DATETIME2 NOT NULL,
    LastLogin DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Accounts Table
CREATE TABLE Accounts (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(255) NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- 'Personal', 'Business'
    CreatedAt DATETIME2 NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Documents Table
CREATE TABLE Documents (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    FileName NVARCHAR(500) NOT NULL,
    SizeBytes BIGINT NOT NULL,
    ContentType NVARCHAR(100),
    CreatedAt DATETIME2 NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id)
);

-- Plans Table
CREATE TABLE Plans (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX),
    PricePerMonth DECIMAL(10,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Subscriptions Table
CREATE TABLE Subscriptions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    AccountId UNIQUEIDENTIFIER NOT NULL,
    PlanId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2,
    Status NVARCHAR(50) NOT NULL, -- 'Active', 'Expired', 'Cancelled'
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    FOREIGN KEY (PlanId) REFERENCES Plans(Id)
);

-- PlanLimits Table
CREATE TABLE PlanLimits (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    PlanId UNIQUEIDENTIFIER NOT NULL,
    ResourceType NVARCHAR(50) NOT NULL, -- 'storage', 'documents', 'users'
    MaxValue BIGINT NOT NULL,
    FOREIGN KEY (PlanId) REFERENCES Plans(Id)
);
```

### Indexes

```sql
-- Performance Indexes
CREATE INDEX IX_Documents_AccountId ON Documents(AccountId) WHERE IsDeleted = 0;
CREATE INDEX IX_Documents_CreatedAt ON Documents(CreatedAt DESC);
CREATE INDEX IX_Subscriptions_AccountId ON Subscriptions(AccountId) WHERE Status = 'Active';
CREATE INDEX IX_Users_Email ON Users(Email);
```

### Entity Framework Configuration

```csharp
public class DocumentStorageDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Document configuration
        modelBuilder.Entity<Document>()
            .HasOne(d => d.Account)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Plan limits configuration
        modelBuilder.Entity<PlanLimit>()
            .HasOne(pl => pl.Plan)
            .WithMany(p => p.PlanLimits)
            .HasForeignKey(pl => pl.PlanId);
    }
}
```

---

## 8. API Documentation

### Authentication Endpoints

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "SecurePassword123!",
    "firstName": "John",
    "lastName": "Doe"
}

Response: 200 OK
{
    "userId": "guid",
    "email": "user@example.com",
    "token": "jwt-token"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
    "email": "user@example.com",
    "password": "SecurePassword123!"
}

Response: 200 OK
{
    "token": "jwt-token",
    "expiresIn": 86400
}
```

### Document Endpoints

#### Upload Document
```http
POST /api/documents/upload
Authorization: Bearer {token}
Content-Type: multipart/form-data

Form Data:
- file: [binary]

Response: 200 OK
{
    "id": "guid",
    "fileName": "document.pdf",
    "sizeBytes": 1024000,
    "contentType": "application/pdf",
    "createdAt": "2025-01-15T10:00:00Z"
}
```

#### Get Documents
```http
GET /api/documents
Authorization: Bearer {token}

Response: 200 OK
[
    {
        "id": "guid",
        "fileName": "document.pdf",
        "sizeBytes": 1024000,
        "createdAt": "2025-01-15T10:00:00Z"
    }
]
```

#### Download Document
```http
GET /api/documents/{id}/download
Authorization: Bearer {token}

Response: 200 OK
Content-Type: application/octet-stream
Content-Disposition: attachment; filename="document.pdf"
[binary data]
```

#### Delete Document
```http
DELETE /api/documents/{id}
Authorization: Bearer {token}

Response: 204 No Content
```

### Plan Endpoints

#### Get Available Plans
```http
GET /api/plans
Authorization: Bearer {token}

Response: 200 OK
[
    {
        "id": "guid",
        "name": "Basic",
        "description": "5 GB storage, 500 documents",
        "pricePerMonth": 9.99,
        "features": ["Email support", "Secure storage"]
    }
]
```

#### Get Current Usage
```http
GET /api/plans/usage
Authorization: Bearer {token}

Response: 200 OK
{
    "storageUsedBytes": 512000000,
    "storageLimit": 1073741824,
    "documentCount": 45,
    "documentLimit": 100,
    "percentageUsed": 47.6
}
```

#### Upgrade Plan
```http
POST /api/plans/upgrade/{planId}
Authorization: Bearer {token}

Response: 200 OK
{
    "success": true,
    "message": "Successfully upgraded to Professional plan",
    "newLimits": {
        "storage": 53687091200,
        "documents": -1
    }
}
```

### Error Responses

#### Validation Error
```json
{
    "error": "VALIDATION_ERROR",
    "message": "Invalid request data",
    "errors": {
        "email": ["Email is required", "Invalid email format"],
        "password": ["Password must be at least 8 characters"]
    }
}
```

#### Authorization Error
```json
{
    "error": "UNAUTHORIZED",
    "message": "Invalid or expired token"
}
```

#### Limit Exceeded Error
```json
{
    "error": "LIMIT_EXCEEDED",
    "message": "Storage limit exceeded for this account",
    "currentUsage": {
        "storageUsedBytes": 1073741824,
        "storageLimit": 1073741824
    },
    "upgradeRequired": true
}
```

---

## 9. User Interface

### Component Architecture

#### Blazor Components Structure
```
Components/
├── Documents/
│   ├── DocumentList.razor       # Document grid display
│   ├── FileUpload.razor         # Drag-drop upload
│   ├── PaywallModal.razor       # Upgrade prompt
│   └── UsageIndicator.razor     # Storage meter
├── Layout/
│   ├── MainLayout.razor         # App shell
│   ├── NavMenu.razor            # Navigation
│   └── Footer.razor             # Footer
├── Pages/
│   ├── Home.razor               # Landing page
│   ├── Documents.razor          # Document manager
│   ├── Plans.razor              # Subscription plans
│   └── Profile.razor            # User profile
└── Shared/
    ├── LoadingSpinner.razor     # Loading indicator
    ├── ErrorBoundary.razor      # Error handling
    └── Toast.razor              # Notifications
```

### Key UI Components

#### FileUpload Component
**Features**:
- Drag and drop support
- Multiple file selection
- Progress tracking
- File type validation
- Size limit checking
- Auto-retry on upgrade

```razor
<FileUpload OnFileUploaded="@HandleFileUploaded"
            MaxFileSize="@MaxFileSize"
            AllowedExtensions="@AllowedExtensions" />
```

#### PaywallModal Component
**Design Elements**:
- Gradient backgrounds
- Animated transitions
- Plan comparison table
- Feature highlights
- One-click upgrade
- Success/failure feedback

```razor
<PaywallModal IsVisible="@ShowPaywall"
              CurrentPlan="@CurrentPlan"
              AvailablePlans="@Plans"
              OnUpgrade="@HandlePlanUpgrade"
              OnClose="@ClosePaywall" />
```

#### DocumentList Component
**Features**:
- Grid/list view toggle
- Sorting options
- Search/filter
- Bulk operations
- Quick actions
- Download functionality

### Styling Approach

#### Tailwind CSS Classes
```html
<!-- Card Component -->
<div class="bg-white rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200 p-6">
    <h3 class="text-lg font-semibold text-gray-900 mb-2">Document Title</h3>
    <p class="text-sm text-gray-600">Document description</p>
</div>

<!-- Button Styles -->
<button class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700
               focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2">
    Upload Document
</button>
```

#### Responsive Design
- Mobile-first approach
- Breakpoint system (sm, md, lg, xl)
- Flexible grid layouts
- Touch-friendly interfaces
- Adaptive navigation

---

## 10. Performance & Caching

### Caching Strategy

#### Memory Cache Implementation
```csharp
public class DocumentService : IDocumentService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public async Task<DocumentDto> GetDocumentByIdAsync(Guid id)
    {
        var cacheKey = $"document_{id}";

        if (_cache.TryGetValue<DocumentDto>(cacheKey, out var cachedDocument))
        {
            _logger.LogInformation($"Cache hit for document {id}");
            return cachedDocument;
        }

        var document = await _documentRepository.GetByIdAsync(id);

        if (document != null)
        {
            var dto = MapToDto(document);
            _cache.Set(cacheKey, dto, _cacheExpiration);
            _logger.LogInformation($"Cached document {id} for {_cacheExpiration.TotalMinutes} minutes");
            return dto;
        }

        return null;
    }
}
```

#### Cache Invalidation
```csharp
public async Task InvalidateDocumentCache(Guid documentId, Guid accountId)
{
    _cache.Remove($"document_{documentId}");
    _cache.Remove($"account_documents_{accountId}");
    _cache.Remove("all_documents");
    _logger.LogInformation($"Cache invalidated for document {documentId}");
}
```

### Performance Optimizations

#### Database Query Optimization
- **Eager Loading**: Include related entities
- **Projection**: Select only required fields
- **Pagination**: Limit result sets
- **Indexing**: Strategic index placement
- **Query Caching**: Cache expensive queries

#### File Storage Optimization
- **Chunked Upload**: Large file handling
- **Compression**: Reduce storage size
- **CDN Integration**: Global distribution
- **Lazy Loading**: Load on demand
- **Thumbnail Generation**: Quick previews

#### API Performance
- **Response Compression**: Gzip/Brotli
- **HTTP/2**: Multiplexing support
- **Connection Pooling**: Reuse connections
- **Async Operations**: Non-blocking I/O
- **Rate Limiting**: Prevent abuse

### Monitoring & Metrics

#### Application Insights
```csharp
services.AddApplicationInsightsTelemetry();

public class DocumentController : ControllerBase
{
    private readonly ILogger<DocumentController> _logger;

    public async Task<IActionResult> UploadDocument(IFormFile file)
    {
        using var activity = Activity.StartActivity("DocumentUpload");
        activity?.SetTag("file.size", file.Length);
        activity?.SetTag("file.name", file.FileName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await _documentService.UploadAsync(file);

            _logger.LogInformation("Document uploaded successfully in {ElapsedMs}ms",
                                  stopwatch.ElapsedMilliseconds);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document upload failed after {ElapsedMs}ms",
                           stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

---

## 11. Deployment Guide

### Development Environment

#### Prerequisites
1. Install .NET 8.0 SDK
2. Install SQL Server 2019+
3. Install Node.js (for Tailwind)
4. Install Visual Studio 2022 or VS Code

#### Local Setup
```bash
# Clone repository
git clone https://github.com/yourusername/DocumentStorage_USV.git
cd DocumentStorage_USV

# Restore packages
dotnet restore

# Update database
cd DocumentStorage.Data
dotnet ef database update

# Run API
cd ../DocumentStorage.API
dotnet run

# Run Client (new terminal)
cd ../DocumentStorage.Client
dotnet run
```

### Production Deployment

#### Azure Deployment

##### App Service Configuration
```yaml
# azure-pipelines.yml
trigger:
- main

pool:
  vmImage: 'windows-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: DotNetCoreCLI@2
  inputs:
    command: 'restore'
    projects: '**/*.csproj'

- task: DotNetCoreCLI@2
  inputs:
    command: 'build'
    projects: '**/*.csproj'
    arguments: '--configuration $(buildConfiguration)'

- task: DotNetCoreCLI@2
  inputs:
    command: 'publish'
    publishWebProjects: true
    arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'

- task: PublishBuildArtifacts@1
```

##### Azure SQL Database
```sql
-- Production database configuration
CREATE DATABASE DocumentStorageDB
WITH
(
    EDITION = 'Standard',
    SERVICE_OBJECTIVE = 'S2',
    MAXSIZE = 10GB
);

-- Configure firewall rules
-- Enable automatic tuning
-- Configure backup retention
```

##### Application Settings
```json
{
  "ConnectionStrings": {
    "Default": "Server=tcp:yourserver.database.windows.net,1433;Database=DocumentStorageDB;User ID=admin;Password=***;Encrypt=True;TrustServerCertificate=False;"
  },
  "JwtSettings": {
    "SecretKey": "***",
    "Issuer": "https://yourdomain.azurewebsites.net",
    "Audience": "DocumentStorage.Client"
  },
  "Azure": {
    "StorageConnectionString": "DefaultEndpointsProtocol=https;AccountName=***",
    "ContainerName": "documents"
  }
}
```

#### Docker Deployment

##### Dockerfile
```dockerfile
# API Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DocumentStorage.API/DocumentStorage.API.csproj", "DocumentStorage.API/"]
COPY ["DocumentStorage.BusinessLayer/DocumentStorage.BusinessLayer.csproj", "DocumentStorage.BusinessLayer/"]
COPY ["DocumentStorage.Data/DocumentStorage.Data.csproj", "DocumentStorage.Data/"]
RUN dotnet restore "DocumentStorage.API/DocumentStorage.API.csproj"
COPY . .
WORKDIR "/src/DocumentStorage.API"
RUN dotnet build "DocumentStorage.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentStorage.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentStorage.API.dll"]
```

##### Docker Compose
```yaml
version: '3.8'
services:
  api:
    build:
      context: .
      dockerfile: DocumentStorage.API/Dockerfile
    ports:
      - "8080:80"
      - "8443:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__Default=Server=db;Database=DocumentStorageDB;User=sa;Password=YourPassword123!
    depends_on:
      - db

  client:
    build:
      context: .
      dockerfile: DocumentStorage.Client/Dockerfile
    ports:
      - "8090:80"
    depends_on:
      - api

  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourPassword123!
    ports:
      - "1433:1433"
    volumes:
      - sql_data:/var/opt/mssql

volumes:
  sql_data:
```

### Environment Configuration

#### Development
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

#### Staging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "AllowedHosts": "staging.yourdomain.com"
}
```

#### Production
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Error"
    }
  },
  "AllowedHosts": "yourdomain.com"
}
```

---

## 12. Testing Strategy

### Unit Testing

#### Service Tests
```csharp
[TestClass]
public class DocumentServiceTests
{
    private Mock<IDocumentRepository> _mockRepository;
    private Mock<ILogger<DocumentService>> _mockLogger;
    private DocumentService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockRepository = new Mock<IDocumentRepository>();
        _mockLogger = new Mock<ILogger<DocumentService>>();
        _service = new DocumentService(_mockRepository.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task CreateDocument_ValidInput_ReturnsDocument()
    {
        // Arrange
        var createDto = new CreateDocumentDto
        {
            FileName = "test.pdf",
            SizeBytes = 1024,
            AccountId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Document>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateDocumentAsync(createDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(createDto.FileName, result.FileName);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Document>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public async Task CreateDocument_ExceedsLimit_ThrowsException()
    {
        // Test storage limit exceeded scenario
    }
}
```

#### Repository Tests
```csharp
[TestClass]
public class DocumentRepositoryTests
{
    private DocumentStorageDbContext _context;
    private DocumentRepository _repository;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DocumentStorageDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DocumentStorageDbContext(options);
        _repository = new DocumentRepository(_context);
    }

    [TestMethod]
    public async Task GetByAccountId_ReturnsFilteredDocuments()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        await _context.Documents.AddRangeAsync(
            new Document { AccountId = accountId, FileName = "doc1.pdf" },
            new Document { AccountId = accountId, FileName = "doc2.pdf" },
            new Document { AccountId = Guid.NewGuid(), FileName = "doc3.pdf" }
        );
        await _context.SaveChangesAsync();

        // Act
        var results = await _repository.GetByAccountIdAsync(accountId);

        // Assert
        Assert.AreEqual(2, results.Count());
    }
}
```

### Integration Testing

#### API Tests
```csharp
[TestClass]
public class DocumentsControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Override with test database
                    services.AddDbContext<DocumentStorageDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

        _client = _factory.CreateClient();
    }

    [TestMethod]
    public async Task UploadDocument_ValidFile_Returns200()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("test content"));
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("text/plain");
        content.Add(fileContent, "file", "test.txt");

        // Act
        var response = await _client.PostAsync("/api/documents/upload", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
```

### End-to-End Testing

#### Selenium Tests
```csharp
[TestClass]
public class DocumentUploadE2ETests
{
    private IWebDriver _driver;
    private WebDriverWait _wait;

    [TestInitialize]
    public void Setup()
    {
        _driver = new ChromeDriver();
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
    }

    [TestMethod]
    public void UploadDocument_CompleteFlow_Success()
    {
        // Navigate to login
        _driver.Navigate().GoToUrl("https://localhost:7215/login");

        // Login
        _driver.FindElement(By.Id("email")).SendKeys("test@example.com");
        _driver.FindElement(By.Id("password")).SendKeys("TestPassword123!");
        _driver.FindElement(By.Id("loginButton")).Click();

        // Wait for navigation
        _wait.Until(d => d.Url.Contains("/documents"));

        // Upload file
        var uploadElement = _driver.FindElement(By.Id("fileUpload"));
        uploadElement.SendKeys(@"C:\test\document.pdf");

        // Verify upload success
        _wait.Until(d => d.FindElement(By.ClassName("upload-success")));

        Assert.IsTrue(_driver.PageSource.Contains("document.pdf"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        _driver.Quit();
    }
}
```

### Performance Testing

#### Load Testing
```csharp
[TestClass]
public class PerformanceTests
{
    [TestMethod]
    public async Task DocumentUpload_ConcurrentRequests_HandlesLoad()
    {
        var tasks = new List<Task<HttpResponseMessage>>();
        var client = GetAuthenticatedClient();

        // Simulate 100 concurrent uploads
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(UploadDocumentAsync(client, $"file{i}.txt"));
        }

        var responses = await Task.WhenAll(tasks);

        var successCount = responses.Count(r => r.IsSuccessStatusCode);
        Assert.IsTrue(successCount > 95, $"Only {successCount} of 100 requests succeeded");
    }
}
```

---

## 13. Troubleshooting

### Common Issues

#### Authentication Problems

**Issue**: "401 Unauthorized" errors
```
Solution:
1. Verify JWT token is included in Authorization header
2. Check token expiration
3. Ensure correct secret key in configuration
4. Verify issuer/audience match
```

**Issue**: "Session expired" during operations
```
Solution:
1. Implement token refresh mechanism
2. Increase token lifetime in settings
3. Add automatic re-authentication
```

#### Upload Failures

**Issue**: "Method Not Allowed" on upload
```
Solution:
1. Ensure endpoint accepts multipart/form-data
2. Add [Consumes("multipart/form-data")] attribute
3. Verify CORS configuration
```

**Issue**: "Limit exceeded" errors
```
Solution:
1. Check current usage vs limits
2. Verify plan subscription status
3. Ensure correct resource type names
4. Clear cache after plan upgrade
```

#### Performance Issues

**Issue**: Slow document listing
```
Solution:
1. Implement pagination
2. Add appropriate indexes
3. Enable caching
4. Use projection to reduce data transfer
```

**Issue**: High memory usage
```
Solution:
1. Configure cache size limits
2. Implement file streaming for large files
3. Use async operations
4. Monitor memory with profiler
```

### Debugging Tools

#### Logging Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

#### Health Checks
```csharp
services.AddHealthChecks()
    .AddDbContextCheck<DocumentStorageDbContext>()
    .AddCheck("Storage", () =>
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        return Directory.Exists(path)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Upload directory not found");
    });
```

---

## 14. Future Roadmap

### Version 2.0 Features

#### Document Versioning
- Track document versions
- Version comparison
- Rollback capability
- Version history UI

#### Team Collaboration
- Share documents with teams
- Permission management
- Comments and annotations
- Real-time collaboration

#### Advanced Search
- Full-text search
- Metadata search
- Tag-based organization
- Search filters

### Version 3.0 Features

#### AI Integration
- Document classification
- Content extraction
- Auto-tagging
- Smart search

#### Mobile Applications
- iOS native app
- Android native app
- Offline sync
- Push notifications

#### Enterprise Features
- SAML/LDAP integration
- Custom branding
- API rate limiting
- Webhooks

### Technical Improvements

#### Infrastructure
- Kubernetes deployment
- Auto-scaling
- Multi-region support
- Disaster recovery

#### Performance
- Redis caching
- CDN integration
- Database sharding
- GraphQL API

#### Security
- Two-factor authentication
- Encryption key management
- Audit logging
- Compliance certifications

---

## Appendices

### A. Configuration Files

#### appsettings.json Template
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=DocumentStorageDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-256-bit-secret-key-here-minimum-32-characters",
    "Issuer": "DocumentStorage.API",
    "Audience": "DocumentStorage.Client",
    "ExpirationHours": 24
  },
  "Storage": {
    "Provider": "FileSystem",
    "Path": "uploads",
    "MaxFileSize": 104857600,
    "AllowedExtensions": [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg"]
  },
  "Cache": {
    "SlidingExpiration": 5,
    "AbsoluteExpiration": 20,
    "Size": 1024
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```

### B. SQL Scripts

#### Initial Data Seed
```sql
-- Insert default plans
INSERT INTO Plans (Id, Name, Description, PricePerMonth, IsActive) VALUES
('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Free', 'Basic features with 1 GB storage', 0.00, 1),
('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Basic', '5 GB storage with email support', 9.99, 1),
('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Professional', '50 GB storage with priority support', 29.99, 1),
('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Enterprise', '500 GB storage with 24/7 support', 99.99, 1);

-- Insert plan limits
INSERT INTO PlanLimits (Id, PlanId, ResourceType, MaxValue) VALUES
(NEWID(), 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'storage', 1073741824), -- 1 GB
(NEWID(), 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'documents', 100),
(NEWID(), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'storage', 5368709120), -- 5 GB
(NEWID(), 'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'documents', 500);
```

### C. API Response Examples

#### Successful Upload
```json
{
  "id": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "accountId": "a8a8a8a8-a8a8-a8a8-a8a8-a8a8a8a8a8a8",
  "fileName": "quarterly-report.pdf",
  "sizeBytes": 2457600,
  "contentType": "application/pdf",
  "createdAt": "2025-01-15T14:30:00Z",
  "accountName": "Acme Corporation"
}
```

#### Usage Statistics
```json
{
  "accountId": "a8a8a8a8-a8a8-a8a8-a8a8-a8a8a8a8a8a8",
  "storageUsedBytes": 524288000,
  "storageLimit": 1073741824,
  "storagePercentage": 48.8,
  "documentCount": 47,
  "documentLimit": 100,
  "documentPercentage": 47.0,
  "planName": "Free",
  "daysUntilRenewal": 15
}
```

### D. Glossary

- **JWT**: JSON Web Token - Secure authentication token
- **DTO**: Data Transfer Object - Object for data transport
- **CORS**: Cross-Origin Resource Sharing - Browser security
- **BCrypt**: Password hashing algorithm
- **Entity Framework**: Object-Relational Mapping framework
- **Blazor**: .NET web UI framework
- **SignalR**: Real-time communication library
- **Repository Pattern**: Data access abstraction
- **Clean Architecture**: Software design philosophy
- **Dependency Injection**: Inversion of control pattern

---

## Contact Information

**Project Lead**: Bogdan Boicu
**Email**: support@documentstorage.com
**Repository**: https://github.com/yourusername/DocumentStorage_USV
**Documentation**: https://docs.documentstorage.com

---

**Document Version**: 1.0.0
**Last Updated**: January 15, 2025
**Next Review**: February 15, 2025