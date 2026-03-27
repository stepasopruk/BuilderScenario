# BuilderScenario 🏗️

[![.NET](https://img.shields.io/badge/.NET-6.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![WPF](https://img.shields.io/badge/UI-WPF-purple)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)

**BuilderScenario** — это десктопное приложение для создания, редактирования и управления сценариями тестирования с микросервисной архитектурой.

---

## 📋 Оглавление

- [Возможности](#-возможности)
- [Архитектура](#-архитектура)
- [Технологии](#-технологии)
- [Начало работы](#-начало-работы)
- [Структура проекта](#-структура-проекта)
- [Использование](#-использование)
- [Тестирование](#-тестирование)
- [Лицензия](#-лицензия)

---

## ✨ Возможности

### Основной функционал
- ✅ **Создание сценариев** — иерархическая структура: Группы → Шаги → Действия
- ✅ **Drag & Drop** — интуитивное переупорядочивание элементов
- ✅ **Валидация данных** — мгновенная проверка и визуальная обратная связь
- ✅ **Сохранение в БД** — SQLite с полной иерархией
- ✅ **Импорт из Word** — поддержка .docx файлов с разметкой `$$$`, `$$`, `$`
- ✅ **Экспорт в JSON** — через отдельный микросервис

### Интерфейс
- 🎨 **Material Design** — современный и приятный внешний вид
- 🔔 **Snackbar уведомления** — ненавязчивые сообщения о действиях
- 🌲 **Дерево навигации** — быстрый переход по элементам
- 📱 **Адаптивная верстка** — удобная работа с большими сценариями

---

## 🏗 Архитектура

Проект построен на **микросервисной архитектуре** с четким разделением ответственности:
┌─────────────────────────────────────────────────────────────┐
│ BuilderScenario.App │
│ WPF Desktop Application │
└─────────────────────────────┬───────────────────────────────┘
│ HTTP
┌─────────────────────────────┼───────────────────────────────┐
│ BuilderScenario.Api │
│ Web API (CRUD, Import) │
└─────────────────────────────┬───────────────────────────────┘
│
┌─────────────────────────────┼───────────────────────────────┐
│ BuilderScenario.ExportService │
│ Microservice (Export) │
└─────────────────────────────────────────────────────────────┘

### Слои приложения

| Проект | Назначение |
|--------|------------|
| **BuilderScenario.App** | WPF UI, ViewModels, Drag & Drop |
| **BuilderScenario.Api** | Web API, контроллеры, AutoMapper |
| **BuilderScenario.ExportService** | Микросервис экспорта |
| **BuilderScenario.Infrastructure** | База данных, репозитории, импорт |
| **BuilderScenario.Core** | Доменные сущности |
| **BuilderScenario.Contracts** | Общие DTO между сервисами |
| **BuilderScenario.Api.Tests** | Интеграционные тесты |

---

## 🛠 Технологии

### Backend
| Технология | Назначение |
|------------|------------|
| .NET 6 / ASP.NET Core | Web API и микросервисы |
| Entity Framework Core | ORM для SQLite |
| AutoMapper | Маппинг Entity ↔ DTO |
| Swagger | Документация API |

### Frontend (WPF)
| Технология | Назначение |
|------------|------------|
| WPF (.NET 6) | Десктопный UI |
| Material Design in XAML | Современный дизайн |
| GongSolutions.Wpf.DragDrop | Drag & Drop |
| MVVM Pattern | Разделение логики и UI |

### Тестирование
| Технология | Назначение |
|------------|------------|
| xUnit | Модульное тестирование |
| FluentAssertions | Выразительные проверки |
| In-memory SQLite | Изолированное тестирование БД |

---

## 🚀 Начало работы

### Требования
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) или JetBrains Rider
- SQLite (встроенный, не требует установки)

### Установка и запуск

1. **Клонирование репозитория**
```bash
git clone https://github.com/yourusername/BuilderScenario.git
cd BuilderScenario
```

2. **Настройка базы данных**
```bash
cd BuilderScenario.Api
dotnet ef database update
```

3. **Запуск микросервисов**

Терминал 1 — API
```bash
cd BuilderScenario.Api
dotnet run
```

4. **Открытие приложения**

API: http://localhost:5120/swagger
Export Service: http://localhost:44332/swagger
WPF: запустится автоматически

## 📁 Структура проекта

BuilderScenario/
├── BuilderScenario.App/                # WPF Desktop Application
│   ├── Common/                         # BaseViewModel, RelayCommand
│   ├── Services/                       # HTTP клиенты
│   ├── ViewModels/                     # MVVM ViewModels
│   └── Views/                          # XAML окна
│
├── BuilderScenario.Api/                # Web API
│   ├── Controllers/                    # ScenariosController, ImportController
│   ├── Dtos/                           # Data Transfer Objects
│   ├── Mapping/                        # AutoMapper профили
│   └── Program.cs                      # Конфигурация
│
├── BuilderScenario.ExportService/      # Microservice
│   ├── Controllers/                    # ExportController
│   ├── Services/                       # ExportService, IExportFormatter
│   └── Middleware/                     # ErrorHandlingMiddleware
│
├── BuilderScenario.Infrastructure/     # Data Access & Import
│   ├── Data/                           # ScenarioDbContext
│   ├── Services/                       # Repository, ImportService
│   └── Services/Import/                # DocxImportParser, IImportParser
│
├── BuilderScenario.Core/               # Domain Entities
│   └── Entities/                       # Scenario, ActionGroup, StepItem, ActionItem
│
├── BuilderScenario.Contracts/          # Shared DTOs
│   └── Export/                         # ExportScenarioDto
│
└── BuilderScenario.Api.Tests/          # Integration Tests
    ├── CustomWebApplicationFactory.cs  # Тестовая фабрика
    └── ScenarioControllerTests.cs      # Тесты API

## 📖 Использование

### Создание сценария

- Нажмите "Создать сценарий"
- Введите название сценария
- Добавьте группы действий (кнопка "Создать новую группу")
- Внутри группы добавьте шаги
- Внутри шага добавьте действия
- Перетаскивайте элементы для изменения порядка
- Нажмите "Сохранить"

### Импорт из Word

Формат документа:
```bash
Название сценария
$$$Группа 1
$$Шаг 1
$Действие 1.1
$Действие 1.2
$$Шаг 2
$Действие 2.1
```

- Нажмите "Загрузить сценарий"
- Выберите .docx файл
- Сценарий автоматически откроется в редакторе

### Экспорт в JSON

- Откройте сценарий
- Нажмите кнопку "Экспорт" (иконка 📤)
- Выберите место сохранения
- Файл будет экспортирован через микросервис

## 🧪 Тестирование

### Запуск тестов
```bash
cd BuilderScenario.Api.Tests
dotnet test
```

### Покрытие тестов
✅ CRUD операции (GET, POST, PUT, DELETE)
✅ Валидация ошибок (404, 400)
✅ Работа с in-memory SQLite
✅ Изоляция тестов (чистая БД перед каждым тестом)

## 📄 Лицензия

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

