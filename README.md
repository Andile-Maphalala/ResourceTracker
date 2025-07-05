-----------------------------------Migrations---------------------------------
dotnet ef migrations add BuidPlanChanges --project ResourceTracker.Persistence --startup-project ResourceTracker.Api
dotnet ef database update --project ResourceTracker.Persistence --startup-project ResourceTracker.Api
dotnet ef migrations remove

---------------------------How to Make secrets-------------------------
mkdir -p ./secrets
cd ./secrets

# Create individual secret files
echo "" > Db__Password.txt
echo "Server=localhost;Port=1111;Database=DB;Username=usr;Password=passsword;" > ConnectionStrings__ConnectionString.txt
echo "FakeName" > JwtSettings__Key.txt
echo "FakeName" > JwtSettings__Issuer.txt
echo "FakeName" > JwtSettings__Audience.txt
echo 1000 > JwtSettings__DurationInMinutes.txt
echo "fake@example.com" > DevCredentials__Email.txt
echo "Password" > DevCredentials__Password.txt
echo "" > ASPNETCORE_Kestrel__Certificates__Default__Password
echo "/secrets/file.pfx" > ASPNETCORE_Kestrel__Certificates__Default__Path
POSTGRES_PASSWORD
Add to gitignore
# Secrets
/secrets/
*.txt