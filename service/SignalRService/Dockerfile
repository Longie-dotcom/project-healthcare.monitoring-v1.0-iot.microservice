# -------------------------------
# Stage 1: Build the .NET projects
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files first and restore
COPY ["Signal/Signal.csproj", "Signal/"]

RUN dotnet restore "Signal/Signal.csproj"

# Copy everything else
COPY . .

# Build the Signal project
WORKDIR /src/Signal
RUN dotnet build "Signal.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------------------------------
# Stage 2: Publish the Signal
# -------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Signal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------------------
# Stage 3: Runtime image
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose ports: HTTP, HTTPS
EXPOSE 5100
EXPOSE 5101

# Copy published files
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "Signal.dll"]
