# Aspire Microservices Project

This project demonstrates a **microservices architecture** built with **.NET 10** and Aspire framework.  

## Services Overview

1. **Catalog Service**
   - Database: PostgreSQL
   - Handles surveys and questions  

2. **Response Service**
   - Depends on: Catalog Service
   - Databases: MongoDB
   - Messaging: RabbitMQ
   - Handles survey responses  

3. **Reporting Service**
   - Messaging: RabbitMQ
   - Database: Redis
   - Generates reports based on responses  

## Requirements
- **Docker** is required to run the services
