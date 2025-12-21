# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файлы проектов для восстановления зависимостей
COPY ["SetRep.API/SetRep.API.csproj", "SetRep.API/"]
COPY ["SetRep.Infrastructure/SetRep.Infrastructure.csproj", "SetRep.Infrastructure/"]
COPY ["SetRep.Core/SetRep.Core.csproj", "SetRep.Core/"]
COPY ["SetRep.Shared/SetRep.Shared.csproj", "SetRep.Shared/"]

# Восстанавливаем зависимости с кэшированием NuGet пакетов
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    --mount=type=cache,id=nuget-http,target=/root/.local/share/NuGet \
    dotnet restore "./SetRep.API/SetRep.API.csproj" --verbosity normal

COPY . .
WORKDIR "/src/SetRep.API"
RUN dotnet build "./SetRep.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SetRep.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SetRep.API.dll"]