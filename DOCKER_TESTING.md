# Docker Testing and Verification Guide

This guide helps you verify that the Docker setup for SBInspector works correctly.

## Pre-flight Checks

Before building, verify:
```bash
# Check Docker is installed and running
docker --version
docker info

# Check you're in the correct directory
pwd  # Should show .../SBInspector

# Verify Dockerfile exists
ls -la Dockerfile
```

## Build Verification

### 1. Build the Docker Image
```bash
docker build -t sbinspector:test .
```

**Expected outcome:**
- Build completes successfully
- No errors in build output
- Final message shows image created

**Verify build:**
```bash
docker images | grep sbinspector
```

Expected output similar to:
```
sbinspector    test    abc123def456    2 minutes ago    250MB
```

### 2. Inspect the Image
```bash
# Check image details
docker inspect sbinspector:test

# Check image layers
docker history sbinspector:test
```

## Runtime Verification

### 3. Start the Container
```bash
docker run -d -p 8080:8080 --name sbinspector-test sbinspector:test
```

**Expected outcome:**
- Container starts successfully
- Returns container ID

### 4. Verify Container is Running
```bash
docker ps | grep sbinspector-test
```

Expected output shows:
- Container ID
- Image name (sbinspector:test)
- Status (Up X seconds)
- Port mapping (0.0.0.0:8080->8080/tcp)

### 5. Check Container Logs
```bash
docker logs sbinspector-test
```

**Expected log entries:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://[::]:8080
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 6. Test Application Access
```bash
# Test HTTP endpoint
curl -I http://localhost:8080

# Or use browser
open http://localhost:8080  # macOS
xdg-open http://localhost:8080  # Linux
start http://localhost:8080  # Windows
```

**Expected outcome:**
- HTTP 200 OK response
- Application loads in browser
- Connection page is displayed

### 7. Verify Container Health
```bash
# Check container stats
docker stats sbinspector-test --no-stream

# Check container processes
docker top sbinspector-test
```

## Docker Compose Verification

### 8. Test with Docker Compose
```bash
# Stop standalone container first
docker stop sbinspector-test
docker rm sbinspector-test

# Start with compose
docker-compose up -d

# Verify service is running
docker-compose ps
```

**Expected output:**
```
Name                Command         State          Ports
---------------------------------------------------------------
sbinspector   dotnet SBInspector.dll   Up      0.0.0.0:8080->8080/tcp
```

### 9. Test with Environment Variables
Create a `.env` file:
```bash
cp .env.example .env
```

Edit `.env` if needed, then:
```bash
docker-compose down
docker-compose up -d
docker-compose logs -f
```

## Functional Testing

### 10. Basic Application Test
1. Navigate to http://localhost:8080
2. You should see the SBInspector home page
3. The connection string input field should be visible
4. UI should be responsive

### 11. Test with Azure Service Bus (Optional)
If you have an Azure Service Bus connection string:
1. Enter the connection string in the application
2. Click "Connect"
3. Verify queues and topics load successfully

## Performance Testing

### 12. Resource Usage
```bash
# Monitor resource usage
docker stats sbinspector

# Check memory limit (should be reasonable)
docker inspect sbinspector | grep -i memory
```

**Expected:**
- Memory usage < 512MB for typical workload
- CPU usage reasonable during idle

### 13. Container Restart Test
```bash
# Test restart capability
docker restart sbinspector
sleep 10
curl -I http://localhost:8080
```

**Expected outcome:**
- Container restarts successfully
- Application becomes available within 10 seconds

## Cleanup and Troubleshooting

### 14. Stop and Remove
```bash
# Stop container
docker stop sbinspector

# Remove container
docker rm sbinspector

# Remove image (optional)
docker rmi sbinspector:test
```

### 15. Clean Build (if needed)
```bash
# Remove all build cache
docker builder prune -a

# Rebuild from scratch
docker build --no-cache -t sbinspector:test .
```

## Common Issues and Solutions

### Build Fails with NuGet Errors

**Problem:** SSL certificate errors during NuGet restore

**Solution:**
1. Check internet connectivity
2. Verify Docker daemon can reach https://api.nuget.org
3. For corporate networks, add CA certificates (see DOCKER.md)

### Container Won't Start

**Problem:** Container exits immediately

**Solution:**
```bash
# Check logs
docker logs sbinspector

# Check for port conflicts
lsof -i :8080  # Linux/macOS
netstat -ano | findstr :8080  # Windows
```

### Cannot Access Application

**Problem:** Can't reach http://localhost:8080

**Solution:**
1. Verify container is running: `docker ps`
2. Check port mapping is correct
3. Check firewall settings
4. Try accessing from inside container:
   ```bash
   docker exec sbinspector curl http://localhost:8080
   ```

### High Memory Usage

**Problem:** Container uses too much memory

**Solution:**
```bash
# Set memory limit
docker run -d -p 8080:8080 --memory="512m" sbinspector:test
```

## Success Criteria Checklist

- [ ] Docker image builds without errors
- [ ] Container starts successfully
- [ ] Application is accessible on http://localhost:8080
- [ ] Logs show no errors or warnings
- [ ] Application UI loads correctly
- [ ] Resource usage is reasonable (< 512MB memory)
- [ ] Container survives restart
- [ ] Docker Compose works correctly
- [ ] Environment variables are respected
- [ ] Container can be stopped and removed cleanly

## Advanced Testing

### Load Testing (Optional)
```bash
# Install Apache Bench (if not installed)
# apt-get install apache2-utils  # Linux
# brew install httpd  # macOS

# Simple load test
ab -n 1000 -c 10 http://localhost:8080/
```

### Security Scanning (Optional)
```bash
# Scan image for vulnerabilities (requires Docker Scout or Trivy)
docker scout cves sbinspector:test

# Or with Trivy
trivy image sbinspector:test
```

## Reporting Issues

If tests fail:
1. Collect logs: `docker logs sbinspector > logs.txt`
2. Collect build output: `docker build --progress=plain . > build.log 2>&1`
3. Check Docker version: `docker --version`
4. Check system resources: `docker info`
5. Include all above in issue report

## Next Steps

After successful verification:
1. Tag image for distribution
2. Push to container registry (optional)
3. Deploy to production environment
4. Set up monitoring and logging
5. Configure automatic updates

## References

- [DOCKER.md](DOCKER.md) - Full deployment documentation
- [DOCKER_QUICKSTART.md](DOCKER_QUICKSTART.md) - Quick reference
- [README.md](README.md) - Application documentation
