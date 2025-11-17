# Monitoring Dashboard

A self-hosted uptime and incident monitoring system built to give you a clear overview of your services. Focused on simplicity, reliability, and clean UI.

## Overview

Monitoring Dashboard helps you track:

* Whether your services are online
* How long incidents last
* When planned maintenance happens
* Daily uptime percentages and trends

Everything runs on .NET 9 with a Blazor and a PostgreSQL database. Background workers continuously check your services and generate daily stats.

## Features

* **Service Uptime Tracking** - Periodic checks with visual indicators.
* **Incident Logging** - Automatic incident creation based on failures.
* **Maintenance Scheduling** - Track planned/unplanned downtime.
* **Daily Statistics** - Uptime, response status aggregation, logs.
* **Role-Based Access** - Admin, User, Viewer roles.
* **Responsive UI** - TailwindCSS-powered, optimized for clarity.

## Architecture

* **Blazor Web App**
* **Background Workers** for monitoring + cleanup
* **ASP.NET Identity** for authentication & authorization
* **Entity Framework Core** for data access
* **PostgreSQL** as database

## Running Locally

1. Configure environment variables (DB connection, ports, etc.)
2. Run EF Core migrations:

```bash
dotnet ef database update
```

3. Start the app:

```bash
dotnet run
```

4. Open the dashboard in your browser on http://localhost:5140
