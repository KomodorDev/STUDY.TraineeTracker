# 🧪 Default Behavior: In-Memory Database

By default, the application uses an **in-memory database** when started via:

```bash
dotnet run
```

This behavior is configured in `Program.cs` and is intended for local development and testing.

---

# 💾 Persistent Database (db.app)

To use the persistent SQLite database stored under `persistence/`, run the app in **Production** mode:

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet run --no-launch-profile
```

---

# ❌ Proper Shutdown Procedure

**Do NOT** use “Kill Terminal” to stop the app.

Instead, press:

```bash
CTRL + C
```

This will gracefully shut down the app and log:

```text
Application is shutting down...
```

---

# 🛠️ Entity Framework Core Setup

To create and apply the initial database migration for the persistent database:

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add InitialCreate
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
```

Make sure these commands are run from the project directory containing the `DbContext`.