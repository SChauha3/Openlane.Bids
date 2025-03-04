# Use the .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install dependencies required for Native AOT
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev

WORKDIR /src

# Copy project files
COPY ["./Openlane.Bids.WorkerService.csproj", "Openlane.Bids.WorkerService/"]
COPY ["../Openlane.Bids.Shared/Openlane.Bids.Shared.csproj", "Openlane.Bids.Shared/"]

# Restore dependencies
RUN dotnet restore "Openlane.Bids.WorkerService/Openlane.Bids.WorkerService.csproj"

# Copy the rest of the application
COPY . .

# Build and Publish as Native AOT
WORKDIR "/src/Openlane.Bids.WorkerService"
RUN dotnet publish "./Openlane.Bids.WorkerService.csproj" \
    -c Release \
    -r linux-x64 \
    -o /app/publish \
    --self-contained true \
    -p:PublishAot=true

# Use a minimal runtime image for the final container
FROM debian:bookworm-slim AS final

# Install required dependencies for the final runtime
RUN apt-get update \
    && apt-get install -y --no-install-recommends libicu72 openssl libssl3 \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the native AOT binary
COPY --from=build /app/publish .

# Set execution permissions
RUN chmod +x /app/Openlane.Bids.WorkerService && ls -l /app/Openlane.Bids.WorkerService

# Ensure correct globalization settings
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
#ENV CLR_OPENSSL_VERSION_OVERRIDE=43

# Set the entrypoint
ENTRYPOINT ["/app/Openlane.Bids.WorkerService"]