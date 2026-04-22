# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/backend

# Copy csproj first (better caching)
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . .

# Publish app
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ResumeATS.dll"]
