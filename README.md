# TaskManagementLab — лабораторная № 9

Готовый учебный проект ASP.NET Core REST API для тестирования по спецификации. Проект специально может содержать нарушения требований. **Цель — обнаружить и описать их; не исправлять API.**

## Требования
- .NET 9 SDK
- Visual Studio 2022 с поддержкой .NET 9 **или** Visual Studio Code с C# / терминал
- Интернет при первом восстановлении NuGet-пакетов

## Запуск

```powershell
dotnet restore
dotnet run --project TaskManagement.Api --launch-profile http
```

Откройте <http://localhost:5089/swagger>. Выберите `GET /api/tasks` → `Try it out` → `Execute`.

## Автоматические тесты

```powershell
dotnet test TaskManagement.Tests/TaskManagement.Tests.csproj
```

Один тест приведён как образец. Добавьте не менее четырёх своих.
