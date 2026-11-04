# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore first with only the project files so the NuGet layer is cached while the code changes.
COPY Directory.Build.props Directory.Packages.props ./
COPY src/Company.Orion.Api/Company.Orion.Api.csproj src/Company.Orion.Api/
COPY src/Company.Orion.Application.Core/Company.Orion.Application.Core.csproj src/Company.Orion.Application.Core/
COPY src/Company.Orion.Domain.Core/Company.Orion.Domain.Core.csproj src/Company.Orion.Domain.Core/
COPY src/Company.Orion.Infra.Data/Company.Orion.Infra.Data.csproj src/Company.Orion.Infra.Data/
COPY src/Company.Orion.Crosscutting.Ioc/Company.Orion.Crosscutting.Ioc.csproj src/Company.Orion.Crosscutting.Ioc/
COPY src/Company.Orion.Crosscutting.Resources/Company.Orion.Crosscutting.Resources.csproj src/Company.Orion.Crosscutting.Resources/
RUN dotnet restore src/Company.Orion.Api/Company.Orion.Api.csproj

COPY . .
RUN dotnet publish src/Company.Orion.Api/Company.Orion.Api.csproj -c Release -o /app/publish --no-restore -p:UseAppHost=false

# ---------- runtime ----------
# Chiseled (distroless-like) Ubuntu image: no shell, no package manager, runs as the non-root "app" user
# and listens on 8080. The "-extra" variant keeps ICU/tzdata, required by the pt-BR/en-US globalization.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Company.Orion.Api.dll"]
