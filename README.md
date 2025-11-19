# Monitoring Dashboard

A self-hosted uptime and incident monitoring system built to give you a clear overview of your services. Focused on simplicity, reliability, and a clean UI.

## Overview

Monitoring Dashboard helps you track:

* Whether your services are online
* How long incidents last
* When planned maintenance happens
* Daily uptime percentages and trends

Everything runs on .NET 9 with Blazor and PostgreSQL. Background workers continuously check your services and generate daily stats.

## Features

* **Service Uptime Tracking** – Periodic checks with visual indicators.
* **Incident Logging** – Automatic incident creation based on failures.
* **Maintenance Scheduling** – Track planned or unplanned downtime.
* **Daily Statistics** – Uptime, response status aggregation, logs.
* **Role-Based Access** – Admin, User, Viewer roles.
* **Responsive UI** – TailwindCSS-powered, optimized for clarity.

## Architecture

* **Blazor Web App**
* **Background Workers** for monitoring + cleanup
* **ASP.NET Identity** for authentication & authorization
* **Entity Framework Core** for data access
* **PostgreSQL** as the database

## Running with Docker Compose

The recommended way to run Monitoring Dashboard is via **Docker Compose**.

### 1. Create a `.env` file

Required fields:
 - `POSTGRES_PASSWORD` - Password for the database user
 - `ADMIN_PASSWORD` - Default admin password for the dashboard (Must contain atleast one alphanumeric and one uppercase letter)

Optional fields:
 - `POSTGRES_DB` - The name of the database (Default: monitoring_dashboard)
 - `POSTGRES_USER` - Username for the database user (Default: monitoring_dashboard)
 - `POSTGRES_HOST` - Host for the database (If you're using the default docker-compose this is optional)
 - `ADMIN_USERNAME` - Default admin username for the dashboard (Default: admin)
 - `ADMIN_EMAIL` - Default admin email for the dashboard (Default: admin@example.com)
 - `APP_NAME` - App name that shows on the dashboard (Default: Monitoring Dashboard)

### 2. Use the provided Docker Compose file

You can find the example Compose file here:

[**docker-compose.yml**](https://github.com/Jakooob14/MonitoringDashboard/blob/master/docker-compose.yml)

It contains the full configuration for the application, PostgreSQL, networks, and data volumes.

### 3. Start the stack

```bash
docker compose up -d
```

### 4. Access the dashboard

Open your browser:

**[http://localhost:8080/dashboard](http://localhost:8080/dashboard)**

The app will automatically create and migrate the PostgreSQL database on first run.
