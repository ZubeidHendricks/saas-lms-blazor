# SAAS Learning Management System (LMS)

A modern, scalable Learning Management System built with Blazor WebAssembly and .NET 8, featuring multi-tenancy, white-labeling capabilities, and offline support through PWA.

## Features

- 🏢 Multi-tenant architecture
- 🎨 White-labeling support
- 📱 Progressive Web App (PWA) with offline capabilities
- 🔐 Azure AD B2C authentication
- 💾 Data synchronization
- 📊 Real-time analytics
- 📚 Comprehensive course management
- 📝 Assessment system
- 📈 Progress tracking

## Tech Stack

- **Frontend**: Blazor WebAssembly
- **Backend**: ASP.NET Core 8
- **Database**: Azure SQL Database
- **Storage**: Azure Blob Storage
- **Authentication**: Azure AD B2C
- **Real-time**: Azure SignalR Service
- **Hosting**: Azure App Service
- **CDN**: Azure CDN

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or later / VS Code with C# extension
- Azure subscription (for cloud services)
- SQL Server (local or cloud)

### Development Setup

1. Clone the repository
```bash
git clone https://github.com/ZubeidHendricks/saas-lms-blazor.git
cd saas-lms-blazor
```

2. Install dependencies
```bash
dotnet restore
```

3. Create development appsettings
```bash
cp src/SaasLMS.Server/appsettings.json src/SaasLMS.Server/appsettings.Development.json
```

4. Update connection strings and configuration in appsettings.Development.json

5. Run migrations
```bash
cd src/SaasLMS.Server
dotnet ef database update
```

6. Run the application
```bash
dotnet run
```

## Solution Structure

```
saas-lms-blazor/
├── src/
│   ├── SaasLMS.Server/        # ASP.NET Core Host
│   ├── SaasLMS.Client/        # Blazor WebAssembly
│   └── SaasLMS.Shared/        # Shared Library
├── tests/
│   ├── SaasLMS.Server.Tests/
│   ├── SaasLMS.Client.Tests/
│   └── SaasLMS.Integration.Tests/
└── tools/                     # Build and deployment tools
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.