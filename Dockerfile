# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y nodejs npm

COPY . .

RUN dotnet restore MonitoringDashboard.sln

WORKDIR /src/MonitoringDashboard
RUN npm ci
RUN npm run build:css

RUN dotnet publish -c Release -o /app/publish


# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "MonitoringDashboard.dll"]
