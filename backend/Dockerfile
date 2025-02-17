# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files and restore dependencies
COPY api/MiniFlexCrmApi/*.csproj ./
RUN dotnet restore

# Copy everything else and build the app
COPY api/MiniFlexCrmApi ./
RUN dotnet publish -c Release -o /publish

# Use the runtime image for final execution
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .
EXPOSE 5000
CMD ["dotnet", "MiniFlexCrmApi.dll"]