@echo
dotnet ef migrations add InitialCreate --startup-project API --project Infrastructure --output-dir Persistence\Migrations
