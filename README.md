# **Orion Api**

[![Build and Test](https://github.com/vanderlan/Orion-Api/actions/workflows/build.yml/badge.svg)](https://github.com/vanderlan/Orion-Api/actions/workflows/build.yml)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=coverage)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api) 
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=alert_status)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=vanderlan_Orion-Api&metric=sqale_index)](https://sonarcloud.io/summary/overall?id=vanderlan_Orion-Api)
[![GitHub release](https://img.shields.io/github/release/vanderlan/Orion-Api.svg)](https://github.com/vanderlan/Orion-Api/releases) 

**About this Project**

*A simple project template for creating a .NET Web Api (v10.0)*

The main objective is to start projects with a clean and simple architecture, without having to redo the entire configuration whenever starting a new project with similar characteristics.

**Libraries**

+ Entity Framework Core *(SqlServer and PostgreSql)*
	+ Fluent API;
	+ CreatedAt and UpdatedAt by default;
	+ Pagination.

+ MongoDB Driver *(MongoDb)*
	+ BsonClassMap mappings;
	+ CreatedAt and UpdatedAt by default;
	+ Indexes and default data created on startup;
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
	14. Logs with Correlation Id;
	15. Relational (SqlServer/PostgreSql) or Document (MongoDb) database.

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
	dotnet new orion-api --companyName "MyCompany" --output "MyProjectName" --systemDatabase "MongoDb"

**Migrations** *(SqlServer and PostgreSql only)*

	# in the src/ folder

	dotnet ef migrations add MigrationName -p Company.Orion.Infra.Data -s Company.Orion.Api
	dotnet ef database update -p Company.Orion.Infra.Data -s Company.Orion.Api --verbose

# **MongoDb**

When the project is created with `--systemDatabase "MongoDb"`, the Entity Framework infrastructure
(DbContext, Fluent API mappings and migrations) is replaced by the MongoDb implementation placed on
`Company.Orion.Infra.Data/Mongo`. The domain contracts (`IUnitOfWork`, `IUserRepository`,
`IRefreshTokenRepository`) are the same, so the application layer does not change.

**Configuration**

	"ConnectionStrings": {
		"MongoDb": "mongodb://mongo:CHANGE_ME_Passw0rd!@localhost:27017/?authSource=admin"
	},
	"MongoDb": {
		"DatabaseName": "OrionDev"
	}

The database name can also be informed on the connection string, e.g. `mongodb://localhost:27017/OrionDev`.

**How it works**

	1. The MongoClient is registered as a singleton (connection pool) and each unit of work owns its own context;
	2. Writes are buffered by the MongoDataContext and only applied on CommitAsync, like the EF Core implementation;
	3. PublicId is mapped as the document _id, and CreatedAt/LastUpdated are filled on commit;
	4. Collections use the entity name (User, RefreshToken);
	5. There are no migrations: the indexes and the default admin user are created on startup (idempotent);
	6. Integration tests run against a throwaway MongoDb container, each fixture using its own database;
	7. The MongoDb data layer is unit tested with a mocked driver (tests/Company.Orion.Test/Unit/Infra/Mongo), no server needed.

*Owner:* https://github.com/vanderlan
