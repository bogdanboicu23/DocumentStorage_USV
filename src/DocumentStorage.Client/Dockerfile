FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY DocumentStorage.Client/DocumentStorage.Client.csproj DocumentStorage.Client/
COPY DocumentStorage.Shared/DocumentStorage.Shared.csproj DocumentStorage.Shared/
RUN dotnet restore DocumentStorage.Client/DocumentStorage.Client.csproj

# Copy everything and build
COPY . .
WORKDIR /src/DocumentStorage.Client
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DocumentStorage.Client.dll"]