# 🌱 Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# 🌐 Set ASP.NET Core URL (Bind to all network interfaces)

# 🚦 Expose Port
Expose 443
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 📂 Copy & Restore
COPY Openlane.Bids.Api/Openlane.Bids.Api.csproj Openlane.Bids.Api/
COPY Openlane.Bids.Shared/Openlane.Bids.Shared.csproj Openlane.Bids.Shared/

# 🚀 Restore project dependencies
RUN dotnet restore Openlane.Bids.Api/Openlane.Bids.Api.csproj

# 📦 Copy all source code
COPY . .
WORKDIR "/src/Openlane.Bids.Api"

# 🛠️ Install Native AOT Prerequisites
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev libicu-dev

# 🚀 Build with Native AOT
RUN dotnet publish "Openlane.Bids.Api.csproj" -c Release -o /app/publish \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:StripSymbols=true

# 🪶 Stage 2: Create Lightweight Native Image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 🎬 Run Native Executable (Ensure executable name is correct)
ENTRYPOINT ["/app/Openlane.Bids.Api"]
