# BUILD Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble-amd64 AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./CloudWeather.Precipitation.csproj" --disable-parallel
RUN dotnet publish "./CloudWeather.Precipitation.csproj" -c Release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0.3-noble-amd64
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "CloudWeather.Precipitation.dll"]
