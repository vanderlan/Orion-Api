FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.11 AS build

EXPOSE 5000

WORKDIR /app

COPY . .

RUN dotnet restore "VBaseProject.Api/VBaseProject.Api.csproj"
RUN dotnet publish  "VBaseProject.Api/VBaseProject.Api.csproj" -c Release -o /out

WORKDIR /out

ENTRYPOINT ["dotnet", "VBaseProject.Api.dll"]