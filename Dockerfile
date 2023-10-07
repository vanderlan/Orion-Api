FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY . ./

RUN dotnet restore "Orion.Api/Orion.Api.csproj"
RUN dotnet publish "Orion.Api/Orion.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80  
EXPOSE 80

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Orion.Api.dll"]