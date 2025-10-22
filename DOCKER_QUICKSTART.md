# Docker Quick Start Guide

This is a quick reference for deploying SBInspector with Docker. For full documentation, see [DOCKER.md](DOCKER.md).

## Quick Commands

### Build the Image
```bash
docker build -t sbinspector:latest .
```

### Run the Container
```bash
docker run -d -p 8080:8080 --name sbinspector sbinspector:latest
```

### Using Docker Compose
```bash
# Start
docker-compose up -d

# Stop
docker-compose down

# View logs
docker-compose logs -f
```

## Access the Application

Open your browser and navigate to:
```
http://localhost:8080
```

## What's Included

- **Dockerfile**: Multi-stage build for optimized image size
- **.dockerignore**: Excludes unnecessary files from build context
- **docker-compose.yml**: Simplified container orchestration
- **.env.example**: Template for environment variables
- **DOCKER.md**: Comprehensive deployment guide

## Image Details

- **Base Image**: mcr.microsoft.com/dotnet/aspnet:9.0
- **Build Image**: mcr.microsoft.com/dotnet/sdk:9.0
- **Exposed Ports**: 8080 (HTTP), 8081 (HTTPS)
- **Default Environment**: Production

## Deployment Options

1. **Local Docker**: Run on your local machine
2. **Docker Hub**: Push to Docker Hub for distribution
3. **Azure Container Instances**: Deploy to Azure
4. **Azure App Service**: Run as Azure Web App
5. **Kubernetes**: Deploy to any Kubernetes cluster

See [DOCKER.md](DOCKER.md) for detailed instructions on each deployment option.
