FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/PortfolioTracker.Api/PortfolioTracker.Api.csproj", "src/PortfolioTracker.Api/"]
COPY ["src/PortfolioTracker.Application/PortfolioTracker.Application.csproj", "src/PortfolioTracker.Application/"]
COPY ["src/PortfolioTracker.Domain/PortfolioTracker.Domain.csproj", "src/PortfolioTracker.Domain/"]
COPY ["src/PortfolioTracker.Infrastructure/PortfolioTracker.Infrastructure.csproj", "src/PortfolioTracker.Infrastructure/"]
RUN dotnet restore "src/PortfolioTracker.Api/PortfolioTracker.Api.csproj"

COPY . .
RUN dotnet publish "src/PortfolioTracker.Api/PortfolioTracker.Api.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PortfolioTracker.Api.dll"]