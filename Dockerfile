FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

WORKDIR /app

COPY . .

RUN dotnet restore "Orion.Api/Orion.Api.csproj"
RUN dotnet publish  "Orion.Api/Orion.Api.csproj" -c Release -o /out

WORKDIR /out

ENTRYPOINT ["dotnet", "Orion.Api.dll"]