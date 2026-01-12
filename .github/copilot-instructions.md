# GitHub Copilot Instructions for This Project

## 🧭 Project Overview

This repository contains a **simple microservices demo** built with:

- **.NET (8/9)**
- **.NET Aspire** for orchestration
- **RabbitMQ** for async messaging
- Optional: **API Gateway** using YARP

The domain is intentionally minimal and consists of:

- **SurveyCatalogService**  
  Manages Applications, Surveys, and Questions.

- **ResponseService**  
  Accepts survey responses (Samples) and publishes an event to RabbitMQ.

- **ReportingService**  
  Listens to `SampleSubmitted` events and updates basic response counts.

The goal:  
Show **clean microservice boundaries**, **separate databases**, **REST via Gateway**, **and an async event pipeline**, orchestrated with **.NET Aspire**.

Everything must remain simple, readable, and demo-friendly.

---

## 🧱 Architectural Conventions (Copilot should follow)

### Service Boundaries

Each service:

- Has its **own .NET project**.
- Has its **own database**.
- Exposes a small **REST API**.
- MUST NOT call other service databases directly.
- Uses **HTTP** only for synchronous calls.
- Uses **RabbitMQ** only for async events.
- Is registered as a project in the Aspire **AppHost**.

### Project Structure

Copilot should generate code following something like:

```text
/SurveyApp.AppHost           # Aspire AppHost (or similar name)
/SurveyApp.ServiceDefaults   # Shared ServiceDefaults project

/SurveyCatalogService
  /Controllers
  /Entities
  /Data
  /DTOs
  /Services
  /Extensions

/ResponseService
  same structure...

/ReportingService
  same structure...

/ApiGateway                  # Optional YARP gateway

/Shared
  /Events
  /Contracts
