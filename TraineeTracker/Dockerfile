FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TraineeTracker.csproj", "./"]
RUN dotnet restore "TraineeTracker.csproj"

# SAH: EF CLI installieren
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
# Ohne das funktionert "RUN dotnet ef database update" nicht

COPY . .

# SAH: Sicherstellen, dass es den Ordner "persistence " gibt
RUN mkdir -p persistence

# RUN dotnet ef database update

RUN dotnet publish "TraineeTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
USER root
RUN chown -R appuser /app

# SAH:
RUN mkdir -p /app/persistence && chown -R appuser /app
# Sicherstellen, dass der Ordner /app/persistence existiert
# und der Benutzer appuser Schreibrechte darauf hat.
# Das ist notwendig, damit EF Core beim Start die lesson.db-Datei
# erfolgreich erstellen und speichern kann – andernfalls kommt es zu
# SQLite Error 14: 'unable to open database file'.

USER appuser
ENTRYPOINT ["dotnet", "TraineeTracker.dll"]