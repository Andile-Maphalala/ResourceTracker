Migrations
dotnet ef migrations add InitialCreate --project ResourceTracker.Persistence --startup-project ResourceTracker.Api
dotnet ef database update --project ResourceTracker.Persistence --startup-project ResourceTracker.Api
dotnet ef migrations remove