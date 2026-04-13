# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/ ./
RUN dotnet restore PayrollCalculator.Client/PayrollCalculator.Client.csproj
RUN dotnet publish PayrollCalculator.Client/PayrollCalculator.Client.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PayrollCalculator.Client.dll"]
