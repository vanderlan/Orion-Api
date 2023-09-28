FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

ENV ASPNETCORE_URLS=http://+:80  

WORKDIR /app

COPY . .

EXPOSE 80

RUN dotnet restore "Orion.Api/Orion.Api.csproj"
RUN dotnet publish  "Orion.Api/Orion.Api.csproj" -c Release -o /out

WORKDIR /out

ENTRYPOINT ["dotnet", "Orion.Api.dll"]