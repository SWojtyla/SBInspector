# GitHub Copilot Instructions for SBInspector

## About This Project

SBInspector is an Azure Service Bus Inspector - available as both a Blazor Server web application and a .NET MAUI desktop application for inspecting Azure Service Bus queues, topics, and messages.

### Project Structure

- **SBInspector** - Blazor Server web application (cross-platform)
- **SEBInspector.Maui** - .NET MAUI desktop application (Windows only)
- **SBInspector.Shared** - Shared Blazor components and business logic

## Code Standards

Follow the clean architecture principles in this repository:

- **Domain Layer** (`Core/Domain/`) - Domain models and entities
- **Application Layer** (`Application/Services/`) - Application services and business logic
- **Infrastructure Layer** (`Infrastructure/ServiceBus/`) - External service implementations
- **Presentation Layer** (`Presentation/Components/`) - UI components and pages

### Architecture Guidelines

1. **Separation of Concerns**: Keep domain logic separate from infrastructure and UI concerns
2. **Dependency Direction**: Dependencies should point inward (Presentation → Application → Domain)
3. **Interface-based Design**: Use interfaces defined in `Core/Interfaces/` for service abstractions
4. **Domain-Driven Design**: Keep domain models in `Core/Domain/` free from infrastructure dependencies

## Key Guidelines

### Feature Documentation

**Keep track of your changes by creating an md file for each feature you implement.**

When implementing a new feature:
1. Create a markdown file in the feature directory describing the feature (e.g., `FILTERING.md`, `SORTING.md`)
2. Include:
   - Feature overview
   - How to use the feature
   - Examples
   - Technical implementation details (if relevant)

### Code Style

1. Use C# 12 features and .NET 9.0 conventions
2. Enable nullable reference types (`<Nullable>enable</Nullable>`)
3. Use implicit usings where appropriate
4. Follow Microsoft's C# coding conventions

### Testing

- Write unit tests for service classes
- Ensure new features are testable
- Mock external dependencies (e.g., Azure Service Bus)
- **Validate changes in both Blazor Server and MAUI applications when applicable**

### Validation Requirements

When making changes that affect the UI or shared components:
1. **Blazor Server Application**: Test with `dotnet run` in the `SBInspector/` directory
2. **MAUI Application**: Build and test on Windows only (MAUI app only supports Windows platform)
   - Use `dotnet build` in the `SEBInspector.Maui/` directory
   - Note: MAUI project will not build on non-Windows platforms

Always validate that changes work correctly in both applications when the change affects shared components.

### Documentation

- Update README.md when adding major features
- Keep XML documentation comments for public APIs
- Document complex algorithms or business logic

## Technology Stack

- **.NET 9.0** - Target framework
- **Blazor Server** - Web UI framework (SBInspector project)
- **.NET MAUI** - Cross-platform desktop framework (SEBInspector.Maui project, Windows only)
- **Azure.Messaging.ServiceBus** - Service Bus SDK
- **C# 12** - Programming language

## Best Practices

1. **Error Handling**: Implement proper error handling and user feedback
2. **Async/Await**: Use async patterns consistently for I/O operations
3. **Dependency Injection**: Leverage built-in DI container
4. **Component Design**: Create reusable Blazor components
5. **State Management**: Keep component state minimal and focused

## Getting Started

When working on this codebase:
1. Review the existing project structure in `SBInspector/`, `SEBInspector.Maui/`, and `SBInspector.Shared/`
2. Understand the clean architecture layers
3. Follow the established patterns in similar components
4. Test locally before committing:
   - For Blazor Server: `dotnet run` in `SBInspector/`
   - For MAUI (Windows only): `dotnet build` in `SEBInspector.Maui/`
5. Create feature documentation as specified above
6. When making changes to shared components, validate in both applications
