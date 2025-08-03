# --------------------------
# BASE IMAGE – RUNTIME ONLY
# --------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000

# Create non-root user and assign permissions
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser


# --------------------------
# BUILD IMAGE – SDK + EF CLI
# --------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy project file and restore dependencies
COPY ["TraineeTracker.csproj", "./"]
RUN dotnet restore "TraineeTracker.csproj"

# Install Entity Framework CLI globally (needed for `dotnet ef database update`)
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy all source files
COPY . .

# Ensure persistence folder exists before publish
RUN mkdir -p persistence

# Publish app without apphost
RUN dotnet publish "TraineeTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false


# --------------------------
# FINAL IMAGE
# --------------------------
FROM base AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .

# Switch to root temporarily to set permissions
USER root
RUN chown -R appuser /app

# Ensure persistence folder exists and is writable by appuser
RUN mkdir -p /app/persistence && chown -R appuser /app
# This is required so EF Core can create/write `lesson.db` without encountering:
# "SQLite Error 14: 'unable to open database file'"

# Switch back to unprivileged user
USER appuser

# Start the application
ENTRYPOINT ["dotnet", "TraineeTracker.dll"]