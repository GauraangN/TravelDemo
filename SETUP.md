# TravelDemo setup

```bash
dotnet new razor -n TravelDemo --auth Individual --use-local-db false
cd TravelDemo
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
```

1. Copy `Models/` and `Data/` from this folder into the project (overwrite the generated `Data/` context).
2. In `Program.cs` set `UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))`
   and in `appsettings.json` use `"DefaultConnection": "Data Source=travel.db"`.
3. Replace `ApplicationDbContext` references with `AppDbContext`.
4. `dotnet ef migrations add Initial && dotnet ef database update`
5. `dotnet run`
