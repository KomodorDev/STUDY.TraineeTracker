Normally Database runs in Memory when starting using "dotnet run" (see Program.cs)

To force using db.app in Persistence, use "
ASPNETCORE_ENVIRONMENT=Production dotnet run --no-launch-profile
"


testuser:
" 
simon.hinterreiter@uni-a.de 
"
" 
Sopro.2025 
"


Program darf erst mit "Kill Terminal" beendet werden wenn. Stattdessen: CTRL + C im Terinmal dann passiert das:
"      Application is shutting down..."



ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add InitalCreate


ASPNETCORE_ENVIRONMENT=Production dotnet ef database update