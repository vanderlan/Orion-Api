FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore "src/Orion.Api/Orion.Api.csproj"
RUN dotnet publish "src/Orion.Api/Orion.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Orion.Api.dll"]