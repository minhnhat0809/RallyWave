FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_URLS=http://+:7006
USER $APP_UID
WORKDIR /app
EXPOSE 7006
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BookingManagement/BookingManagement.csproj", "BookingManagement/"]
RUN dotnet restore "BookingManagement/BookingManagement.csproj"
COPY . .
WORKDIR "/src/BookingManagement"
RUN dotnet build "BookingManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BookingManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingManagement.dll"]
