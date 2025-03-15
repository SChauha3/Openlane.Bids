# 🌱 Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 🌐 Set ASP.NET Core URL (Bind to all network interfaces)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# 📂 Copy all source code properly
COPY src/Openlane.Bids.Api/ Openlane.Bids.Api/
COPY src/Openlane.Bids.Shared/ Openlane.Bids.Shared/

# 🚀 Restore project dependencies
WORKDIR /src/Openlane.Bids.Api
RUN dotnet restore Openlane.Bids.Api.csproj

# 🛠️ Install Native AOT Prerequisites
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev libicu-dev

# 🚀 Build with Native AOT
RUN dotnet publish "Openlane.Bids.Api.csproj" -c Release -o /app/publish \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:StripSymbols=true

# 🪶 Stage 2: Runtime Image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 🎬 Run Native Executable
ENTRYPOINT ["/app/Openlane.Bids.Api"]
