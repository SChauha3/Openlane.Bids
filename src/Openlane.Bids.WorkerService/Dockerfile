# Use the .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install dependencies required for Native AOT
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev

WORKDIR /src

# ✅ Correctly copy project files (entire folders)
COPY src/Openlane.Bids.WorkerService/ Openlane.Bids.WorkerService/
COPY src/Openlane.Bids.Shared/ Openlane.Bids.Shared/

# ✅ Restore dependencies (from correct relative path)
WORKDIR /src/Openlane.Bids.WorkerService
RUN dotnet restore Openlane.Bids.WorkerService.csproj

# ✅ Build and Publish as Native AOT
RUN dotnet publish "Openlane.Bids.WorkerService.csproj" \
    -c Release \
    -r linux-x64 \
    -o /app/publish \
    --self-contained true \
    -p:PublishAot=true

# Minimal runtime image for final stage
FROM debian:bookworm-slim AS final

# Install runtime dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends libicu72 openssl libssl3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the built app
COPY --from=build /app/publish .

# Make sure binary is executable
RUN chmod +x /app/Openlane.Bids.WorkerService && ls -l /app/Openlane.Bids.WorkerService

# Globalization support
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Entry point
ENTRYPOINT ["/app/Openlane.Bids.WorkerService"]
