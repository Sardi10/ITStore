# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy all files to /src
COPY . .

# Navigate into the folder containing the .csproj file
WORKDIR /src/NewarkITStore

# Restore and publish the project
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use ASP.NET runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/wwwroot/images
ENTRYPOINT ["dotnet", "NewarkITStore.dll"]
