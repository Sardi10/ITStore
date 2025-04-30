# Use the official .NET 8 SDK image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# Change directory and restore from correct path
WORKDIR /src/NewarkITStore
RUN dotnet restore "NewarkITStore.csproj"
RUN dotnet publish "NewarkITStore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "NewarkITStore.dll"]
