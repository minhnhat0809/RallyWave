FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_URLS=http://+:7004
USER $APP_UID
WORKDIR /app
EXPOSE 7004
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["NotificationManagement/NotificationManagement.csproj", "NotificationManagement/"]
RUN dotnet restore "NotificationManagement/NotificationManagement.csproj"
COPY . .
WORKDIR "/src/NotificationManagement"
RUN dotnet build "NotificationManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationManagement.dll"]
