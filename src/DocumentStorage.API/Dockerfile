FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY DocumentStorage.API/DocumentStorage.API.csproj DocumentStorage.API/
COPY DocumentStorage.BusinessLayer/DocumentStorage.BusinessLayer.csproj DocumentStorage.BusinessLayer/
COPY DocumentStorage.Data/DocumentStorage.Data.csproj DocumentStorage.Data/
COPY DocumentStorage.Shared/DocumentStorage.Shared.csproj DocumentStorage.Shared/
RUN dotnet restore DocumentStorage.API/DocumentStorage.API.csproj

# Copy everything and build
COPY . .
WORKDIR /src/DocumentStorage.API
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Create uploads directory
RUN mkdir -p /app/uploads

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DocumentStorage.API.dll"]