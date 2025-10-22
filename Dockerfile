# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY SBInspector.sln ./
COPY SBInspector/SBInspector.csproj SBInspector/

# Restore dependencies
RUN dotnet restore SBInspector/SBInspector.csproj

# Copy the rest of the source code
COPY . .

# Build and publish the application
WORKDIR /src/SBInspector
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published files from build stage
COPY --from=build /app/publish .

# Set environment variables for production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SBInspector.dll"]
