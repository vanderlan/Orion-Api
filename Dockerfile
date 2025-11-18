FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore "src/Company.Orion.Api/Company.Orion.Api.csproj"
RUN dotnet publish "src/Company.Orion.Api/Company.Orion.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Company.Orion.Api.dll"]