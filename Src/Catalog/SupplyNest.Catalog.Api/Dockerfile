FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Src/Catalog/SupplyNest.Domaine/SupplyNest.Domaine.csproj", "Src/Catalog/SupplyNest.Domaine/"]
RUN dotnet restore "Src/Catalog/SupplyNest.Domaine/SupplyNest.Domaine.csproj"
COPY . .
WORKDIR "/src/Src/Catalog/SupplyNest.Domaine"
RUN dotnet build "./SupplyNest.Domaine.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SupplyNest.Domaine.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SupplyNest.Domaine.dll"]
