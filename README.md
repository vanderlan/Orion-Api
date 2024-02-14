# **Orion Api**

[![Build](https://github.com/vanderlan/Orion-Api/actions/workflows/sonar.yml/badge.svg)](https://github.com/vanderlan/Orion-Api/actions/workflows/sonar.yml) 
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=coverage)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api) 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=alert_status)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=sqale_index)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![Maintainability](https://api.codeclimate.com/v1/badges/76a30970ddd45c75129b/maintainability)](https://codeclimate.com/github/vanderlan/Orion-Api/maintainability) 
[![GitHub release](https://img.shields.io/github/release/vanderlan/Orion-Api.svg)](https://github.com/vanderlan/Orion-Api/releases) 

**About this Project**

*A simple project template for creating a .NET Web Api (v8.0)*

The main objective is to start projects with a clean and simple architecture, without having to redo the entire configuration whenever starting a new project with similar characteristics.

**Libraries**

+ Entity Framework Core
	+ Fluent API;
	+ CreatedAt and UpdatedAt by default;
	+ Pagination.

+ MediaR
+ Swagger
+ Fluent Validation
+ Authentication and Authorization
	+ JWT Token;
	+ Claims and profiles configuration;
	+ Personalized decorators;
	+ Refresh Token.

+ Serilog
+ Bogus

**Configurations and Patterns**

	1. Business Exceptions;
	2. Exception Middleware;
	3. Repository Pattern;
	4. Faker Objects;
	5. Unit Of Work;
	6. Base Repository;
	7. Environments configuration;
	8. CORS Configuration;
	9. Async API methods;
	10. Docker and Docker-Compose;
	11. API Version Configuration (by x-api-version header attribute);
	12. Globalization;
	13. In-Memory database for Testing;
	14. Mediator + CQRS + Notification Pattern;
	15. Logs with Correlation Id.

**CI & CD**

	1. Unit, Integration and Api Tests;
	2. Continuous Integration (GitHub CI);
	3. Continuous Delivery (GitHub CI);
	4. DockerHub Integration;
	5. Sonar Cloud Integration.


# **Create your Project based on the Orion Api Template**

**Install template and create your project**

	dotnet new install .

	dotnet new orion-api -o MyNewProject

**Migrations**

	# in the src/ folder

	dotnet ef migrations add MigrationName -p Orion.Infra.Data -s Orion.Api
	dotnet ef database update -p Orion.Infra.Data -s Orion.Api --verbose

*author:* https://github.com/vanderlan