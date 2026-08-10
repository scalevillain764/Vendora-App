# 1. Берем SDK для сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 2. Копируем csproj и делаем restore
COPY ["Vendora.csproj", "./"]
RUN dotnet restore

# 3. Копируем все файлы и собираем проект
COPY . .
RUN dotnet publish "Vendora.csproj" -c Release -o /app/publish

# 4. Берем минимальный runtime для запуска
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 5. Команда запуска приложения
ENTRYPOINT ["dotnet", "Vendora.dll"]