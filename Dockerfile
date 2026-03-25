# ===============================
# Payway Service Dockerfile
# .NET 10 — Multi-stage build
# ===============================

# -------------------------------
# Stage 1: Build
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project file and restore dependencies (layer cache)
COPY payway.csproj ./
RUN dotnet restore

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish payway.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# -------------------------------
# Stage 2: Runtime
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime

WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# ASP.NET Core listens on 8080 by default (.NET 8+)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "payway.dll"]
