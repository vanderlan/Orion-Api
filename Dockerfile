FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
EXPOSE 5000

WORKDIR /app

COPY . .

RUN dotnet restore "VBaseProject.Api/VBaseProject.Api.csproj"

WORKDIR /app/VBaseProject.Api

RUN dotnet build "VBaseProject.Api.csproj" -c Release -o /app
RUN dotnet publish "VBaseProject.Api.csproj" -c Release -o /app

WORKDIR /app

RUN dotnet --version
ENTRYPOINT ["dotnet", "VBaseProject.Api.dll"]

#docker build -t backend .
#docker run -p 8080:5000 backend
#PORTS ext:docker