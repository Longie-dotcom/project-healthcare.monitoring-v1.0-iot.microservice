# -------------------------------
# Stage 1: Build the .NET projects
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files first and restore
COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

RUN dotnet restore "API/API.csproj"

# Copy everything else
COPY . .

# Build the API project
WORKDIR /src/API
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------------------------------
# Stage 2: Publish the API
# -------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------
# Stage 3: Runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose ports: HTTP, HTTPS, gRPC
EXPOSE 5000
EXPOSE 5001
EXPOSE 5002

# Copy published files
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "API.dll"]
