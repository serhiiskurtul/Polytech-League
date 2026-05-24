# Етап збірки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо всі файли рішень та проєктів
COPY ["src/PolytechLeague.API/PolytechLeague.API.csproj", "src/PolytechLeague.API/"]
COPY ["src/PolytechLeague.Application/PolytechLeague.Application.csproj", "src/PolytechLeague.Application/"]
COPY ["src/PolytechLeague.Domain/PolytechLeague.Domain.csproj", "src/PolytechLeague.Domain/"]
COPY ["src/PolytechLeague.Infrastructure/PolytechLeague.Infrastructure.csproj", "src/PolytechLeague.Infrastructure/"]
COPY ["src/PolytechLeague.Persistence/PolytechLeague.Persistence.csproj", "src/PolytechLeague.Persistence/"]

# Відновлюємо залежності
RUN dotnet restore "src/PolytechLeague.API/PolytechLeague.API.csproj"

# Копіюємо весь вихідний код
COPY . .

# Збираємо проєкт
WORKDIR "/app/src/PolytechLeague.API"
RUN dotnet publish "PolytechLeague.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Фінальний етап (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render автоматично надає порт через змінну PORT, .NET має її слухати
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PolytechLeague.API.dll"]
