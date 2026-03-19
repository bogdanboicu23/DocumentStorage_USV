# DocumentStorage System

## 📚 Overview

DocumentStorage is a comprehensive cloud-based document management system built with modern .NET technologies. It provides secure file storage, user management, subscription-based plans, and a responsive web interface for managing documents.

## 🏗️ System Architecture

### **Clean Architecture Pattern**
The system implements Clean Architecture principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────┐
│                   Presentation Layer                │
│  ┌─────────────────┐        ┌──────────────────┐  │
│  │  Blazor Client  │        │    MVC Admin    │  │
│  └─────────────────┘        └──────────────────┘  │
└─────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────┐
│                     API Layer                       │
│  ┌──────────────────────────────────────────────┐ │
│  │            ASP.NET Core Web API              │ │
│  │         (DocumentStorage.API)                │ │
│  └──────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────┐
│                  Business Layer                     │
│  ┌──────────────────────────────────────────────┐ │
│  │      DocumentStorage.BusinessLayer           │ │
│  │         (Services & Business Logic)          │ │
│  └──────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────┐
│                    Data Layer                       │
│  ┌──────────────────────────────────────────────┐ │
│  │         DocumentStorage.Data                 │ │
│  │    (Repositories & Database Context)         │ │
│  └──────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────┐
│                    Database                         │
│  ┌──────────────────────────────────────────────┐ │
│  │            SQL Server Database               │ │
│  └──────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

## 📁 Project Structure

```
DocumentStorage_USV/
├── DocumentStorage.API/           # Web API project
│   ├── Controllers/              # API endpoints
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Services/                # API-specific services
│   └── Program.cs               # Application configuration
│
├── DocumentStorage.BusinessLayer/ # Business logic layer
│   ├── Services/                # Business services
│   │   ├── AuthService.cs      # Authentication logic
│   │   ├── DocumentService.cs  # Document management
│   │   ├── PlanService.cs      # Subscription plans
│   │   └── UsageService.cs     # Usage tracking
│   └── Interfaces/             # Service interfaces
│
├── DocumentStorage.Client/       # Blazor WebAssembly client
│   ├── Components/              # Blazor components
│   │   ├── Documents/          # Document-related components
│   │   ├── Layout/            # Layout components
│   │   └── Pages/             # Page components
│   ├── Models/                 # Client-side models
│   ├── Services/              # Client services
│   └── wwwroot/               # Static files
│
├── DocumentStorage.Data/         # Data access layer
│   ├── Models/                 # Entity models
│   ├── Repositories/           # Repository implementations
│   ├── Interfaces/            # Repository interfaces
│   └── DocumentStorageDbContext.cs # EF Core context
│
├── DocumentStorage.MVC/          # MVC Admin panel
│   ├── Controllers/           # MVC controllers
│   ├── Views/                # Razor views
│   └── Models/               # View models
│
└── DocumentStorage.Shared/      # Shared DTOs and models
    └── DTOs/                   # Shared DTOs
```

## 🚀 Features

### **Core Features**
- 📄 **Document Management**: Upload, download, delete, and organize documents
- 👤 **User Authentication**: JWT-based authentication with secure login/registration
- 💳 **Subscription Plans**: Tiered subscription system (Free, Basic, Professional, Enterprise)
- 📊 **Usage Tracking**: Real-time monitoring of storage and document count
- 🚫 **Paywall System**: Automatic limits enforcement with upgrade prompts
- 💾 **Caching**: Memory caching for improved performance
- 🔒 **Security**: Role-based access control and secure file storage

### **Technical Features**
- **Clean Architecture**: Separation of concerns with layered architecture
- **Repository Pattern**: Abstracted data access layer
- **Dependency Injection**: IoC container for loose coupling
- **Entity Framework Core**: Code-first database approach
- **JWT Authentication**: Secure token-based authentication
- **Memory Caching**: Performance optimization with IMemoryCache
- **Responsive Design**: Mobile-friendly Blazor components
- **Real-time Updates**: Live usage statistics and limits

## 🛠️ Technology Stack

### **Backend**
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **Database**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: JWT Bearer Tokens
- **Password Hashing**: BCrypt.Net
- **Caching**: IMemoryCache
- **API Documentation**: Swagger/OpenAPI

### **Frontend**
- **Blazor Server**: Interactive web UI
- **CSS Framework**: Tailwind CSS
- **JavaScript Interop**: For file downloads
- **Component Architecture**: Reusable Blazor components

### **Development Tools**
- **IDE**: Visual Studio 2022 / Visual Studio Code
- **Package Manager**: NuGet
- **Version Control**: Git

## 📋 Prerequisites

- .NET 8.0 SDK or higher
- SQL Server 2019 or higher
- Visual Studio 2022 or VS Code
- Node.js (for Tailwind CSS compilation)

## 🔧 Installation & Setup

### 1. **Clone the Repository**
```bash
git clone https://github.com/yourusername/DocumentStorage_USV.git
cd DocumentStorage_USV
```

### 2. **Configure Database Connection**
Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER;Database=DocumentStorageDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. **Configure JWT Settings**
Update JWT configuration in `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_HERE_MINIMUM_32_CHARACTERS",
    "Issuer": "DocumentStorage.API",
    "Audience": "DocumentStorage.Client"
  }
}
```

### 4. **Run Database Migrations**
```bash
cd DocumentStorage.Data
dotnet ef database update
```

### 5. **Start the API Server**
```bash
cd DocumentStorage.API
dotnet run
```
The API will be available at `https://localhost:7150`

### 6. **Start the Blazor Client**
```bash
cd DocumentStorage.Client
dotnet run
```
The client will be available at `https://localhost:7215`

## 🔑 Authentication & Authorization

### **JWT Token Flow**
1. User registers/logs in via `/api/auth/login`
2. Server validates credentials and generates JWT token
3. Client stores token and includes it in subsequent requests
4. API validates token for protected endpoints

### **User Roles**
- **Owner**: Full access to account and documents
- **Member**: Limited access based on permissions
- **Admin**: System-wide administrative access

## 💳 Subscription Plans

### **Plan Tiers**

| Plan | Storage | Documents | Price | Features |
|------|---------|-----------|-------|----------|
| **Free** | 1 GB | 100 | $0 | Basic features |
| **Basic** | 5 GB | 500 | $9.99/mo | Email support, Secure storage |
| **Professional** | 50 GB | Unlimited | $29.99/mo | Priority support, Versioning, Team collaboration |
| **Enterprise** | 500 GB | Unlimited | $99.99/mo | 24/7 support, Custom integrations, API access |

### **Usage Enforcement**
- Real-time usage tracking
- Automatic paywall display when limits reached
- Seamless upgrade flow with immediate activation
- File retry after successful upgrade

## 🗄️ Database Schema

### **Core Tables**
- `Users`: User accounts and authentication
- `Accounts`: Organization/personal accounts
- `Documents`: Document metadata and references
- `Plans`: Subscription plan definitions
- `Subscriptions`: Active subscriptions
- `PlanLimits`: Resource limits per plan
- `AccountUsers`: User-account relationships

### **Key Relationships**
- Users ↔ Accounts (Many-to-Many via AccountUsers)
- Accounts → Documents (One-to-Many)
- Accounts → Subscriptions (One-to-Many)
- Subscriptions → Plans (Many-to-One)
- Plans → PlanLimits (One-to-Many)

## 🚦 API Endpoints

### **Authentication**
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login

### **Documents**
- `GET /api/documents` - Get all documents
- `GET /api/documents/{id}` - Get document by ID
- `POST /api/documents/upload` - Upload document
- `GET /api/documents/{id}/download` - Download document
- `DELETE /api/documents/{id}` - Delete document

### **Plans**
- `GET /api/plans` - Get available plans
- `GET /api/plans/current` - Get current user plan
- `GET /api/plans/usage` - Get usage statistics
- `POST /api/plans/upgrade/{planId}` - Upgrade to plan

## 🔐 Security Considerations

1. **Password Security**: BCrypt hashing with salt
2. **JWT Security**: Secure key storage, token expiration
3. **CORS Policy**: Restricted to specific origins
4. **File Security**: Files stored with GUID prefixes
5. **SQL Injection**: Parameterized queries via EF Core
6. **XSS Protection**: Input validation and encoding

## 🎨 UI Components

### **Key Components**
- **FileUpload**: Drag-and-drop file upload with progress
- **DocumentList**: Grid view of documents with actions
- **PaywallModal**: Beautiful upgrade prompt with plan comparison
- **FloatingAuthButton**: Persistent auth status indicator
- **NavMenu**: Responsive navigation with auth-aware items

## ⚡ Performance Optimizations

1. **Memory Caching**
   - Document metadata cached for 5 minutes
   - File content cached for files < 5MB
   - Cache invalidation on create/delete

2. **Lazy Loading**
   - Documents loaded on demand
   - Pagination for large document lists

3. **Efficient Queries**
   - Includes for related data
   - Projection for DTOs
   - Async/await throughout

## 📊 Monitoring & Logging

- **Structured Logging**: ILogger with categorized log levels
- **Cache Hit Logging**: Track cache effectiveness
- **Error Logging**: Comprehensive error tracking
- **Performance Metrics**: Response time monitoring

## 🧪 Testing Considerations

### **Unit Testing**
- Service layer tests
- Repository layer tests
- Controller tests with mocked dependencies

### **Integration Testing**
- API endpoint testing
- Database integration tests
- Authentication flow tests

## 📝 Future Enhancements

1. **File Versioning**: Track document versions
2. **Team Collaboration**: Share documents with teams
3. **Advanced Search**: Full-text search capabilities
4. **File Preview**: In-browser document preview
5. **Bulk Operations**: Multi-file upload/download
6. **Audit Trail**: Comprehensive activity logging
7. **2FA**: Two-factor authentication
8. **API Rate Limiting**: Prevent abuse
9. **Webhooks**: Event notifications
10. **Mobile App**: Native mobile clients

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👨‍💻 Authors

- **Bogdan Boicu** - Initial work and architecture

## 🙏 Acknowledgments

- ASP.NET Core team for the excellent framework
- Blazor team for the innovative web UI framework
- Entity Framework Core team for the powerful ORM
- Tailwind CSS for the utility-first CSS framework

## 📞 Support

For support, email support@documentstorage.com or open an issue in the GitHub repository.

---

**Last Updated**: January 2025
**Version**: 1.0.0