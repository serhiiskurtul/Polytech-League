# Етап збірки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо всі файли рішень та проєктів
COPY ["backend/src/PolytechLeague.API/PolytechLeague.API.csproj", "backend/src/PolytechLeague.API/"]
COPY ["backend/src/PolytechLeague.Application/PolytechLeague.Application.csproj", "backend/src/PolytechLeague.Application/"]
COPY ["backend/src/PolytechLeague.Domain/PolytechLeague.Domain.csproj", "backend/src/PolytechLeague.Domain/"]
COPY ["backend/src/PolytechLeague.Infrastructure/PolytechLeague.Infrastructure.csproj", "backend/src/PolytechLeague.Infrastructure/"]
COPY ["backend/src/PolytechLeague.Persistence/PolytechLeague.Persistence.csproj", "backend/src/PolytechLeague.Persistence/"]

# Відновлюємо залежності
RUN dotnet restore "backend/src/PolytechLeague.API/PolytechLeague.API.csproj"

# Копіюємо весь вихідний код
COPY . .

# Збираємо проєкт
WORKDIR "/app/backend/src/PolytechLeague.API"
RUN dotnet publish "PolytechLeague.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Фінальний етап (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render автоматично надає порт через змінну PORT, .NET має її слухати
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PolytechLeague.API.dll"]
