dotnet tool update --global dotnet-ef --version 6.0.4
dotnet build
dotnet ef --startup-project ../PEX.Api/ database update --context ApplicationDbContext
pause