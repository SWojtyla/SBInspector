# Docker Deployment Guide for SBInspector

This guide explains how to build and run SBInspector using Docker.

## Overview

SBInspector provides a Dockerfile for containerized deployment. The Docker image uses a multi-stage build process to create an optimized production image.

## Prerequisites

- Docker installed on your system
- Azure Service Bus connection string

## Building the Docker Image

Navigate to the repository root directory and build the image:

```bash
docker build -t sbinspector:latest .
```

This will create a Docker image named `sbinspector` with the `latest` tag.

### Build Process

The Dockerfile uses a multi-stage build:

1. **Build Stage**: Uses the `mcr.microsoft.com/dotnet/sdk:9.0` image to build the application
   - Restores NuGet packages
   - Compiles the .NET application
   - Publishes the release build

2. **Runtime Stage**: Uses the lightweight `mcr.microsoft.com/dotnet/aspnet:9.0` image
   - Copies only the published application files
   - Configures the runtime environment
   - Exposes port 8080

## Running the Container

### Basic Usage

Run the container with the following command:

```bash
docker run -d -p 8080:8080 --name sbinspector sbinspector:latest
```

The application will be available at `http://localhost:8080`

### Running with Environment Variables

You can configure the application using environment variables:

```bash
docker run -d \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --name sbinspector \
  sbinspector:latest
```

### Accessing the Application

1. Open your browser and navigate to `http://localhost:8080`
2. Enter your Azure Service Bus connection string on the home page
3. Click "Connect" to start inspecting your Service Bus entities

## Container Management

### Stop the Container

```bash
docker stop sbinspector
```

### Start the Container

```bash
docker start sbinspector
```

### Remove the Container

```bash
docker rm sbinspector
```

### View Container Logs

```bash
docker logs sbinspector
```

### Follow Container Logs

```bash
docker logs -f sbinspector
```

## Docker Compose (Optional)

You can also create a `docker-compose.yml` file for easier management:

```yaml
version: '3.8'

services:
  sbinspector:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped
```

Then use:

```bash
# Start the application
docker-compose up -d

# Stop the application
docker-compose down

# View logs
docker-compose logs -f
```

## Configuration

### Ports

- **8080**: HTTP port (default)
- **8081**: HTTPS port (exposed but requires certificate configuration)

### Environment Variables

- `ASPNETCORE_URLS`: URL bindings (default: `http://+:8080`)
- `ASPNETCORE_ENVIRONMENT`: Environment name (Production, Development, Staging)

## Security Considerations

### HTTPS Configuration

For production deployments with HTTPS:

1. Generate or obtain an SSL certificate
2. Mount the certificate into the container
3. Configure the certificate in the application settings

Example with certificate:

```bash
docker run -d \
  -p 8080:8080 \
  -p 8443:8081 \
  -v /path/to/cert:/https:ro \
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/cert.pfx \
  -e ASPNETCORE_Kestrel__Certificates__Default__Password=YourCertPassword \
  --name sbinspector \
  sbinspector:latest
```

### Connection String Security

**Important**: Never hardcode Azure Service Bus connection strings in the Docker image. Users should enter the connection string through the web interface after the container starts.

For automated scenarios, you could:
- Use Azure Managed Identity (when running in Azure)
- Store connection strings in Azure Key Vault
- Use Docker secrets (in Swarm mode)

## Troubleshooting

### Container won't start

Check the logs:
```bash
docker logs sbinspector
```

### Cannot access the application

Verify the port mapping:
```bash
docker ps
```

Ensure port 8080 is not already in use on your host machine.

### Build fails with NuGet errors

If you encounter SSL certificate errors during build, ensure your Docker daemon has access to the internet and can reach https://api.nuget.org.

For corporate environments with SSL inspection:
- Configure NuGet to use your corporate certificate
- Add your corporate CA certificate to the Docker build

Example for adding custom CA certificate:

```dockerfile
# Add this after the FROM line in the build stage
RUN apt-get update && apt-get install -y ca-certificates
COPY your-ca-cert.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates
```

## Continuous Integration/Deployment

### GitHub Actions Example

Create `.github/workflows/docker-build.yml`:

```yaml
name: Docker Build and Push

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Build Docker image
      run: docker build -t sbinspector:${{ github.sha }} .
    
    - name: Test Docker image
      run: |
        docker run -d --name test-container -p 8080:8080 sbinspector:${{ github.sha }}
        sleep 10
        curl -f http://localhost:8080 || exit 1
        docker stop test-container
    
    - name: Login to Docker Hub
      if: github.event_name == 'push'
      uses: docker/login-action@v2
      with:
        username: ${{ secrets.DOCKER_USERNAME }}
        password: ${{ secrets.DOCKER_PASSWORD }}
    
    - name: Tag and Push
      if: github.event_name == 'push'
      run: |
        docker tag sbinspector:${{ github.sha }} yourusername/sbinspector:latest
        docker push yourusername/sbinspector:latest
```

## Advanced Deployment

### Deploying to Azure Container Instances

```bash
# Login to Azure
az login

# Create a resource group
az group create --name sbinspector-rg --location eastus

# Create container instance
az container create \
  --resource-group sbinspector-rg \
  --name sbinspector \
  --image sbinspector:latest \
  --dns-name-label sbinspector-unique \
  --ports 8080
```

### Deploying to Azure App Service

1. Build and push your image to Azure Container Registry:

```bash
# Create Azure Container Registry
az acr create --name myregistry --resource-group sbinspector-rg --sku Basic

# Login to ACR
az acr login --name myregistry

# Tag your image
docker tag sbinspector:latest myregistry.azurecr.io/sbinspector:latest

# Push to ACR
docker push myregistry.azurecr.io/sbinspector:latest
```

2. Create an App Service:

```bash
az webapp create \
  --resource-group sbinspector-rg \
  --plan myappserviceplan \
  --name sbinspector \
  --deployment-container-image-name myregistry.azurecr.io/sbinspector:latest
```

### Deploying to Kubernetes

Create a deployment manifest (`k8s-deployment.yaml`):

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: sbinspector
spec:
  replicas: 1
  selector:
    matchLabels:
      app: sbinspector
  template:
    metadata:
      labels:
        app: sbinspector
    spec:
      containers:
      - name: sbinspector
        image: sbinspector:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
---
apiVersion: v1
kind: Service
metadata:
  name: sbinspector
spec:
  type: LoadBalancer
  ports:
  - port: 80
    targetPort: 8080
  selector:
    app: sbinspector
```

Deploy:

```bash
kubectl apply -f k8s-deployment.yaml
```

## Performance Tuning

### Resource Limits

When running in production, set resource limits:

```bash
docker run -d \
  -p 8080:8080 \
  --memory="512m" \
  --cpus="0.5" \
  --name sbinspector \
  sbinspector:latest
```

### Health Checks

Add a health check to your container:

```bash
docker run -d \
  -p 8080:8080 \
  --health-cmd='curl -f http://localhost:8080/ || exit 1' \
  --health-interval=30s \
  --health-timeout=10s \
  --health-retries=3 \
  --name sbinspector \
  sbinspector:latest
```

## Image Size Optimization

The current image uses a multi-stage build which significantly reduces the final image size:

- Build image: ~2GB (includes SDK)
- Runtime image: ~200-300MB (only includes runtime)

To check your image size:

```bash
docker images sbinspector
```

## Updating the Application

To update to a new version:

```bash
# Pull latest code and rebuild
git pull
docker build -t sbinspector:latest .

# Stop and remove old container
docker stop sbinspector
docker rm sbinspector

# Run new container
docker run -d -p 8080:8080 --name sbinspector sbinspector:latest
```

## Support

For issues or questions:
- Check the main [README.md](README.md) for application features
- Review Docker logs for error messages
- Ensure Azure Service Bus connection string is valid
