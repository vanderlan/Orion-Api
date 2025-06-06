# **Orion Api**

[![Build and Test](https://github.com/vanderlan/Orion-Api/actions/workflows/build.yml/badge.svg)](https://github.com/vanderlan/Orion-Api/actions/workflows/build.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=coverage)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api) 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=alert_status)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=sqale_index)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![GitHub release](https://img.shields.io/github/release/vanderlan/Orion-Api.svg)](https://github.com/vanderlan/Orion-Api/releases) 

**About this Project**

*A simple project template for creating a .NET Web Api (v9.0)*

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
	9. Docker and Docker-Compose;
	10. API Versioning Configuration (by x-api-version header attribute);
	11. Globalization;
	12. Real database on CI Tests;
	13. Mediator + CQRS + Notification Pattern;
	14. Logs with Correlation Id.

**CI & CD**

	1. Unit, Integration and Api Tests;
	2. Continuous Integration (GitHub CI);
	3. Continuous Delivery (GitHub CI);
	4. DockerHub Integration;
	5. Sonar Cloud Integration.


# **Create your Project based on the Orion Api Template**

**Install template and create your project**

	dotnet new install .
	dotnet new orion-api --companyName "MyCompany" --output "MyProjectName" --systemDatabase "PostgreSql"
	dotnet new orion-api --companyName "MyCompany" --output "MyProjectName" --systemDatabase "SqlServer"

**Migrations**

	# in the src/ folder

	dotnet ef migrations add MigrationName -p Company.Orion.Infra.Data -s Company.Orion.Api
	dotnet ef database update -p Company.Orion.Infra.Data -s Company.Orion.Api --verbose

*Owner:* https://github.com/vanderlan
