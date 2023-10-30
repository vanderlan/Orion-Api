# **Orion Api**

[![Build](https://github.com/vanderlan/Orion-Api/actions/workflows/sonar.yml/badge.svg)](https://github.com/vanderlan/Orion-Api/actions/workflows/sonar.yml) 
[![Coverage Status](https://coveralls.io/repos/github/vanderlan/Orion-Api/badge.svg)](https://coveralls.io/github/vanderlan/Orion-Api) <a href="https://codeclimate.com/github/vanderlan/Orion-Api/maintainability"><img src="https://api.codeclimate.com/v1/badges/76a30970ddd45c75129b/maintainability" /></a>
[![GitHub release](https://img.shields.io/github/release/vanderlan/Orion-Api.svg)](https://GitHub.com/vanderlan/Orion-Api/) 
[![GitHub repo size](https://img.shields.io/github/repo-size/vanderlan/Orion-Api)](https://github.com/vanderlan/Orion-Api)

**About this Project**

*A simple project template for creating a .NET API (v7.0)*

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
	14. Mediator + CQRS + Notification Pattern
	15. Logs with Correlation Id

**CI & CD**

	1. Automated tests;
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

	dotnet ef migrations add MigrationName -p  Orion.Infra.Data -s Orion.Api
	dotnet ef database update -p Orion.Infra.Data -s Orion.Api --verbose

Author: https://github.com/vanderlan