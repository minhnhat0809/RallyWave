FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_URLS=http://+:7003
USER $APP_UID
WORKDIR /app
EXPOSE 7003
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CourtManagement/CourtManagement.csproj", "CourtManagement/"]
RUN dotnet restore "CourtManagement/CourtManagement.csproj"
COPY . .
WORKDIR "/src/CourtManagement"
RUN dotnet build "CourtManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CourtManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CourtManagement.dll"]
