# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy project file and restore dependencies (cached layer)
COPY TaskTrackerAPI.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Build the release binary
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
# Lightweight ASP.NET runtime image (no SDK = smaller attack surface)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskTrackerAPI.dll"]
