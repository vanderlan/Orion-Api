# **Orion**

[![Build and test](https://github.com/vanderlan/Orion-Api/actions/workflows/dotnet.yml/badge.svg)](https://github.com/vanderlan/Orion-Api/actions/workflows/dotnet.yml) [![Coverage Status](https://coveralls.io/repos/github/vanderlan/Orion-Api/badge.svg)](https://coveralls.io/github/vanderlan/Orion-Api) <a href="https://codeclimate.com/github/vanderlan/Orion-Api/maintainability"><img src="https://api.codeclimate.com/v1/badges/692bddf02255681fe599/maintainability" /></a> [![GitHub release](https://img.shields.io/github/release/vanderlan/Orion-Api.svg)](https://GitHub.com/vanderlan/Orion-Api/) [![GitHub repo size](https://img.shields.io/github/repo-size/vanderlan/Orion-Api)](https://github.com/vanderlan/Orion-Api)

**About this Project**

*A project reference for creating REST APIs with C# and .NET (v7.0).*

The main objective is to start projects with a clean and simple architecture, without having to redo the entire configuration whenever starting a new project with similar characteristics.

**Libraries**

+ Entity Framework Core
	+ Fluent API;
	+ CreatedAt and UpdatedAt by default;
	+ Pagination;
	+ Optimized for best performance.

+ AutoMapper
	+ Unmapped members Configuration.
+ Swagger
+ Fluent Validation
+ Microsoft Identity Authentication
	+ JWT Token;
	+ Claims and profiles configuration;
	+ Personalized decorators;
	+ Refresh Token.

+ NLog

**Configurations and Patterns**

	1. Business Exceptions;
	2. Exception handler;
	3. Repository Pattern;
	4. Mother Objects;
	5. Unit Of Work;
	6. Base Repository;
	7. Environments configuration;
	8. CORS Configuration;
	9. Async API methods;
	10. Docker and Docker-Compose;
	11. API Version Configuration (by x-api-version header attribute);
	12. Globalization;
	13. In-Memory database for Testing.

**CI & CD**

	1. Automated tests;
	2. Continuous Integration;
	3. Continuous Delivery;
	4. DockerHub Integration.

**Todo**

	1. API Test.

**Run database migrations**
	
	dotnet ef database update -p Orion.Data -s Orion.Api --verbose