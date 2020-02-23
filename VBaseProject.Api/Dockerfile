FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

COPY ["VBaseProject.Api/VBaseProject.Api.csproj", "VBaseProject.Api/"]
COPY ["VBaseProject.Service/VBaseProject.Service.csproj", "VBaseProject.Service/"]
COPY ["VBaseProject.Entities/VBaseProject.Entities.csproj", "VBaseProject.Entities/"]
COPY ["VBaseProject.Resources/VBaseProject.Resources.csproj", "VBaseProject.Resources/"]
COPY ["VBaseProject.Data/VBaseProject.Data.csproj", "VBaseProject.Data/"]

RUN dotnet restore "VBaseProject.Api/VBaseProject.Api.csproj"

COPY . .
WORKDIR "/src/VBaseProject.Api"
RUN dotnet build "VBaseProject.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VBaseProject.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VBaseProject.Api.dll"]