# SetRep - Документация проекта

## Обзор

SetRep - это комплексное решение для отслеживания тренировок и прогресса, состоящее из нескольких взаимосвязанных проектов. Система построена на архитектуре Clean Architecture с разделением на слои: Core (доменная модель), Infrastructure (доступ к данным), API (REST API), Bot (Telegram бот) и MiniApp (веб-приложение).

## Архитектура решения

Решение использует многослойную архитектуру с четким разделением ответственности:

- **SetRep.Core** - Доменная модель и бизнес-логика
- **SetRep.Infrastructure** - Реализация доступа к данным (EF Core, PostgreSQL)
- **SetRep.Shared** - Общие DTO и модели для обмена данными
- **SetRep.API** - REST API для работы с данными
- **SetRep.Bot** - Telegram бот для регистрации пользователей
- **SetRep.MiniApp** - Blazor Server хост-приложение
- **SetRep.MiniApp.Client** - Blazor WebAssembly клиентское приложение

## Технологический стек

### Backend
- **.NET 9.0 / 10.0** - Основная платформа
- **ASP.NET Core** - Веб-фреймворк
- **Entity Framework Core 10.0** - ORM
- **PostgreSQL 16** - База данных
- **JWT Bearer Authentication** - Аутентификация
- **Telegram.Bot 22.7.5** - Интеграция с Telegram
- **AWSSDK.S3** - Работа с Yandex Object Storage (S3-совместимое API)
- **SixLabors.ImageSharp** - Обработка изображений
- **Magick.NET** - Дополнительная обработка изображений

### Frontend
- **Blazor WebAssembly 9.0** - Клиентский фреймворк
- **Blazor Server** - Серверный хост
- **MudBlazor 8.x** - UI компоненты
- **JavaScript** - Интеграция с Telegram WebApp API

### Инфраструктура
- **Docker** - Контейнеризация
- **Docker Compose** - Оркестрация контейнеров
- **Yandex Object Storage** - Хранилище изображений

---

## Проекты решения

### 1. SetRep.Core

**Назначение:** Доменная модель и бизнес-логика приложения.

**Целевой фреймворк:** .NET 9.0

**Зависимости:**
- SetRep.Shared

**Основные компоненты:**

#### Сущности (Entities)
- **User** - Пользователь приложения
  - `Id` (Guid) - Уникальный идентификатор
  - `TelegramId` (long) - ID пользователя в Telegram
  - `Name` (string) - Имя пользователя
  - `Username` (string) - Username в Telegram
  - `CreatedAt` (DateTime) - Дата создания
  - `UpdatedAt` (DateTime?) - Дата обновления
  - Навигационные свойства: `Workouts`, `MeasurementsHistory`, `WorkoutPlans`

- **Exercise** - Упражнение
  - `Id` (Guid) - Уникальный идентификатор
  - `Name` (string) - Название упражнения
  - `Image` (string) - Гифка упражнения
  - `MainImage` (string?) - Главная картинка для карточек
  - `VideoUrl` (string?) - Ссылка на видео
  - `Description` (string?) - Описание
  - `MuscleGroup` (MuscleGroup?) - Группа мышц
  - `WeightUnit` (string) - Единица измерения веса (по умолчанию "кг")
  - `IsSystem` (bool) - Системное упражнение (доступно всем)
  - `CreatedByUserId` (Guid?) - ID создателя
  - `CreatedAt` (DateTime) - Дата создания
  - `UpdatedAt` (DateTime?) - Дата обновления

- **Workout** - Тренировка
  - `Id` (Guid) - Уникальный идентификатор
  - `UserId` (Guid) - ID пользователя
  - `WorkoutPlanId` (Guid?) - ID плана тренировки (опционально)
  - `Name` (string) - Название тренировки
  - `Notes` (string?) - Заметки
  - `CreatedAt` (DateTime) - Дата создания
  - `UpdatedAt` (DateTime?) - Дата обновления
  - Навигационные свойства: `User`, `WorkoutPlan`, `WorkoutExercises`

- **WorkoutExercise** - Упражнение в тренировке
  - `Id` (Guid) - Уникальный идентификатор
  - `WorkoutId` (Guid) - ID тренировки
  - `ExerciseId` (Guid) - ID упражнения
  - `Order` (int) - Порядок выполнения
  - `Notes` (string?) - Заметки к упражнению
  - Навигационные свойства: `Workout`, `Exercise`, `Sets`

- **Set** - Подход
  - `Id` (Guid) - Уникальный идентификатор
  - `WorkoutExerciseId` (Guid) - ID упражнения в тренировке
  - `Reps` (int?) - Количество повторений
  - `Weight` (decimal?) - Вес
  - `Duration` (int?) - Длительность (секунды)
  - `Distance` (decimal?) - Дистанция (метры)
  - `Order` (int) - Порядок подхода
  - `CreatedAt` (DateTime) - Дата создания
  - Навигационное свойство: `WorkoutExercise`

- **WorkoutPlan** - План тренировок
  - `Id` (Guid) - Уникальный идентификатор
  - `UserId` (Guid) - ID пользователя
  - `Name` (string) - Название плана
  - `Description` (string?) - Описание
  - `CreatedAt` (DateTime) - Дата создания
  - `UpdatedAt` (DateTime?) - Дата обновления
  - Навигационные свойства: `User`, `Workouts`

- **WorkoutHistory** - История выполненных тренировок
  - `Id` (Guid) - Уникальный идентификатор
  - `UserId` (Guid) - ID пользователя
  - `WorkoutId` (Guid) - ID тренировки
  - `StartedAt` (DateTime) - Время начала
  - `CompletedAt` (DateTime?) - Время завершения
  - `Notes` (string?) - Заметки
  - Навигационные свойства: `User`, `Workout`, `WorkoutHistorySets`

- **WorkoutHistorySet** - Подход в истории тренировки
  - `Id` (Guid) - Уникальный идентификатор
  - `WorkoutHistoryId` (Guid) - ID истории тренировки
  - `ExerciseId` (Guid) - ID упражнения
  - `Reps` (int?) - Количество повторений
  - `Weight` (decimal?) - Вес
  - `Duration` (int?) - Длительность
  - `Distance` (decimal?) - Дистанция
  - `Order` (int) - Порядок подхода
  - Навигационные свойства: `WorkoutHistory`, `Exercise`

- **MeasurementsHistory** - История измерений (вес, объемы)
  - `Id` (Guid) - Уникальный идентификатор
  - `UserId` (Guid) - ID пользователя
  - `Weight` (decimal?) - Вес (кг)
  - `Chest` (decimal?) - Обхват груди (см)
  - `Waist` (decimal?) - Обхват талии (см)
  - `Hips` (decimal?) - Обхват бедер (см)
  - `Biceps` (decimal?) - Обхват бицепса (см)
  - `Thigh` (decimal?) - Обхват бедра (см)
  - `Neck` (decimal?) - Обхват шеи (см)
  - `Date` (DateTime) - Дата измерения
  - `Notes` (string?) - Заметки
  - Навигационное свойство: `User`

#### Перечисления (Enums)
- **MuscleGroup** - Группы мышц:
  - Chest (Грудь)
  - Back (Спина)
  - Shoulders (Плечи)
  - Biceps (Бицепс)
  - Triceps (Трицепс)
  - Quadriceps (Квадрицепсы)
  - Hamstrings (Бицепс бедра)
  - Glutes (Ягодицы)
  - Calves (Икры)
  - Abs (Пресс)
  - Forearms (Предплечья)
  - Traps (Трапеции)
  - Cardio (Кардио)
  - Other (Другое)

#### Репозитории (Repositories)
Интерфейсы репозиториев для работы с данными:
- `IRepository<T>` - Базовый интерфейс репозитория
- `IUserRepository` - Работа с пользователями
- `IExerciseRepository` - Работа с упражнениями
- `IWorkoutRepository` - Работа с тренировками
- `IWorkoutPlanRepository` - Работа с планами тренировок
- `IWorkoutExerciseRepository` - Работа с упражнениями в тренировках
- `ISetRepository` - Работа с подходами
- `IMeasurementsHistoryRepository` - Работа с историей измерений
- `IWorkoutHistoryRepository` - Работа с историей тренировок
- `IWorkoutHistorySetRepository` - Работа с подходами в истории

#### Сервисы API (ApiRequestServices)
Интерфейсы для клиентских сервисов:
- `IUserService`
- `IExerciseService`
- `IWorkoutService`
- `IWorkoutPlanService`
- `IWorkoutExerciseService`
- `ISetService`
- `IMeasurementsHistoryService`
- `IWorkoutHistoryService`
- `IWorkoutHistorySetService`
- `IFileUploadService`

#### Расширения (Extensions)
- `EnumExtensions` - Расширения для работы с перечислениями

#### Атрибуты (Attributes)
- `ImageUrlAttribute` - Атрибут для указания URL изображения

---

### 2. SetRep.Infrastructure

**Назначение:** Реализация доступа к данным и инфраструктурных сервисов.

**Целевой фреймворк:** .NET 10.0

**Зависимости:**
- SetRep.Core
- Npgsql.EntityFrameworkCore.PostgreSQL 10.0.0
- Microsoft.EntityFrameworkCore.Design 10.0.0
- Microsoft.EntityFrameworkCore.Tools 10.0.0

**Основные компоненты:**

#### Контекст базы данных
- **ApplicationDbContext** - DbContext для работы с PostgreSQL
  - `Users` - Пользователи
  - `Exercises` - Упражнения
  - `Workouts` - Тренировки
  - `WorkoutExercises` - Упражнения в тренировках
  - `Sets` - Подходы
  - `MeasurementsHistory` - История измерений
  - `WorkoutHistory` - История тренировок
  - `WorkoutHistorySets` - Подходы в истории
  - `WorkoutPlans` - Планы тренировок

#### Конфигурации сущностей (Data/Configurations)
Конфигурации EF Core для всех сущностей:
- `UserConfiguration`
- `ExerciseConfiguration`
- `WorkoutConfiguration`
- `WorkoutExerciseConfiguration`
- `SetConfiguration`
- `WorkoutPlanConfiguration`
- `WorkoutHistoryConfiguration`
- `WorkoutHistorySetConfiguration`
- `MeasurementsHistoryConfiguration`

#### Репозитории (Repositories)
Реализации репозиториев:
- `Repository<T>` - Базовый репозиторий
- `UserRepository`
- `ExerciseRepository`
- `WorkoutRepository`
- `WorkoutPlanRepository`
- `WorkoutExerciseRepository`
- `SetRepository`
- `MeasurementsHistoryRepository`
- `WorkoutHistoryRepository`
- `WorkoutHistorySetRepository`

#### Миграции (Migrations)
Миграции базы данных находятся в папке `Migrations/`.

---

### 3. SetRep.Shared

**Назначение:** Общие DTO и модели для обмена данными между проектами.

**Целевой фреймворк:** .NET 9.0

**Зависимости:** Нет

**Основные компоненты:**

#### DTO (Data Transfer Objects)
- `UserDto` - DTO пользователя
- `CreateWorkoutDto` - DTO для создания тренировки
- `CreateWorkoutPlanDto` - DTO для создания плана тренировок
- `CreateWorkoutExerciseDto` - DTO для добавления упражнения в тренировку
- `CreateSetDto` - DTO для создания подхода
- `CreateWorkoutHistoryDto` - DTO для создания записи истории тренировки
- `CreateWorkoutHistorySetDto` - DTO для создания подхода в истории
- `CreateMeasurementsHistoryDto` - DTO для создания записи измерений

#### Telegram модели
- `TelegramUser` - Модель пользователя Telegram
- `TelegramWebAppData` - Данные Telegram WebApp

---

### 4. SetRep.API

**Назначение:** REST API для работы с данными тренировок.

**Целевой фреймворк:** .NET 10.0

**Тип проекта:** ASP.NET Core Web API

**Зависимости:**
- SetRep.Infrastructure
- SetRep.Shared
- Microsoft.AspNetCore.Authentication.JwtBearer 10.0.1
- Microsoft.AspNetCore.OpenApi 9.0.11
- Swashbuckle.AspNetCore 9.0.6
- AWSSDK.S3 4.0.17.2
- SixLabors.ImageSharp 3.1.12

**Основные компоненты:**

#### Контроллеры (Controllers)
- **AuthController** (`/api/auth`)
  - `POST /api/auth/login` - Аутентификация через Telegram WebApp данные, возвращает JWT токен

- **UsersController** (`/api/users`)
  - Работа с пользователями

- **ExercisesController** (`/api/exercises`)
  - CRUD операции для упражнений
  - Получение системных и пользовательских упражнений

- **WorkoutsController** (`/api/workouts`)
  - CRUD операции для тренировок

- **WorkoutPlansController** (`/api/workoutplans`)
  - CRUD операции для планов тренировок

- **WorkoutExercisesController** (`/api/workoutexercises`)
  - Управление упражнениями в тренировках

- **SetsController** (`/api/sets`)
  - CRUD операции для подходов

- **WorkoutHistoryController** (`/api/workouthistory`)
  - Работа с историей тренировок

- **WorkoutHistorySetsController** (`/api/workouthistorysets`)
  - Работа с подходами в истории тренировок

- **MeasurementsHistoryController** (`/api/measurementshistory`)
  - CRUD операции для истории измерений

- **FileUploadController** (`/api/fileupload`)
  - Загрузка и удаление изображений в Yandex Object Storage

- **TelegramWebAppController** (`/api/telegramwebapp`)
  - Валидация данных Telegram WebApp

#### Сервисы (Services)
- **IJwtTokenService / JwtTokenService**
  - Генерация и валидация JWT токенов
  - Токены содержат: `userId`, `telegramId`, `username`
  - Время жизни токена: 24 часа (настраивается)

- **ITelegramWebAppValidationService / TelegramWebAppValidationService**
  - Валидация данных Telegram WebApp
  - Проверка подписи данных с использованием токена бота

- **IYandexObjectStorageService / YandexObjectStorageService**
  - Загрузка изображений в Yandex Object Storage
  - Удаление изображений
  - Использует S3-совместимое API

- **WorkoutAutoCompletionService** (HostedService)
  - Фоновый сервис для автоматического завершения незавершенных тренировок
  - Запускается при старте приложения

#### Расширения (Extensions)
- **ClaimsPrincipalExtensions**
  - Расширения для получения данных пользователя из JWT токена
  - `GetUserId()`, `GetTelegramId()`, `GetUsername()`

#### Опции (Options)
- **FileUploadOptions**
  - Настройки загрузки файлов:
    - `MaxFileSizeBytes` - Максимальный размер файла
    - `MaxUploadsPerUser` - Максимальное количество загрузок на пользователя

#### Конфигурация
- **Аутентификация:** JWT Bearer Authentication
- **База данных:** PostgreSQL через EF Core
- **CORS:** Настроен для доступа из MiniApp и WebApp
- **Swagger:** Доступен в режиме Development
- **Миграции:** Применяются автоматически при старте приложения

#### Переменные окружения
- `ConnectionStrings__DefaultConnection` - Строка подключения к PostgreSQL
- `BotToken` - Токен Telegram бота
- `Jwt__SecretKey` - Секретный ключ для JWT (минимум 32 символа)
- `Jwt__Issuer` - Издатель JWT токена
- `Jwt__Audience` - Аудитория JWT токена
- `Jwt__ExpirationMinutes` - Время жизни токена в минутах
- `YANDEX_OBJECT_STORAGE_ACCESS_KEY` - Access key для Yandex Object Storage
- `YANDEX_OBJECT_STORAGE_SECRET_KEY` - Secret key для Yandex Object Storage
- `YANDEX_OBJECT_STORAGE_ENDPOINT` - Endpoint Yandex Object Storage
- `YANDEX_OBJECT_STORAGE_BUCKET_NAME` - Имя бакета
- `YANDEX_OBJECT_STORAGE_SERVICE_URL` - URL сервиса
- `FileUpload__MaxFileSizeBytes` - Максимальный размер файла
- `FileUpload__MaxUploadsPerUser` - Максимальное количество загрузок

---

### 5. SetRep.Bot

**Назначение:** Telegram бот для регистрации пользователей и интеграции с Telegram MiniApp.

**Целевой фреймворк:** .NET 9.0

**Тип проекта:** ASP.NET Core Web API

**Зависимости:**
- Telegram.Bot 22.7.5
- Microsoft.AspNetCore.OpenApi 9.0.11

**Основные компоненты:**

#### Endpoints
- `GET /bot/setWebhook` - Установка webhook для бота
- `POST /bot` - Обработка обновлений от Telegram

#### Функциональность
- **Обработка команды /start**
  - Проверка существования пользователя в API
  - Создание нового пользователя, если не существует
  - Отправка приветственного сообщения
  - Интеграция с API через HTTP клиент

#### Конфигурация
- `BotToken` - Токен Telegram бота (обязательно)
- `BotWebhookUrl` - URL для webhook (обязательно)
- `ApiBaseUrl` - Базовый URL API (по умолчанию: `http://localhost:5181`)

#### Особенности
- Использует типизированный HTTP клиент для работы с API
- Обрабатывает только текстовые сообщения
- Логирует все операции для отладки

---

### 6. SetRep.MiniApp

**Назначение:** Серверный хост для Blazor WebAssembly приложения.

**Целевой фреймворк:** .NET 9.0

**Тип проекта:** ASP.NET Core Blazor Server

**Зависимости:**
- SetRep.Core
- SetRep.Shared
- SetRep.MiniApp.Client
- Microsoft.AspNetCore.Components.WebAssembly.Server 9.*
- Magick.NET-Q16-AnyCPU 14.10.2

**Основные компоненты:**

#### Сервисы
- **IImageConversionService / ImageConversionService**
  - Конвертация изображений с использованием Magick.NET
  - Используется для обработки изображений перед загрузкой

#### Расширения
- **ServiceCollectionExtensions**
  - Регистрация сервисов для MiniApp

#### Опции
- **FileUploadOptions**
  - Настройки загрузки файлов

#### Конфигурация
- Настроен для хостинга Blazor WebAssembly приложения
- Интеграция с Telegram WebApp

---

### 7. SetRep.MiniApp.Client

**Назначение:** Клиентское Blazor WebAssembly приложение для работы с тренировками.

**Целевой фреймворк:** .NET 9.0

**Тип проекта:** Blazor WebAssembly

**Зависимости:**
- SetRep.Core
- SetRep.Shared
- Microsoft.AspNetCore.Components.WebAssembly 9.*
- Microsoft.Extensions.Http 10.0.1
- MudBlazor 8.*

**Основные компоненты:**

#### Страницы (Pages)
- **Home.razor** - Главная страница
- **Profile.razor** - Профиль пользователя
- **Statistics.razor** - Статистика тренировок

##### Упражнения (Pages/Exercise)
- **Exercises.razor** - Список упражнений
- **ExerciseDetail.razor** - Детали упражнения
- **ExerciseGroup.razor** - Группировка упражнений по группам мышц

##### Тренировки (Pages/Workout)
- **Workouts.razor** - Список тренировок
- **AddWorkout.razor** - Добавление тренировки
- **WorkoutDetail.razor** - Детали тренировки
- **ActiveWorkout.razor** - Активная тренировка
- **WorkoutExerciseDetail.razor** - Детали упражнения в тренировке
- **WorkoutHistoryPage.razor** - История тренировок
- **WorkoutHistoryDetail.razor** - Детали истории тренировки
- **AddWorkoutPlan.razor** - Добавление плана тренировок

##### Измерения (Pages/Measurements)
- **MeasurementsList.razor** - Список измерений
- **AddMeasurement.razor** - Добавление измерения
- **MeasurementDetail.razor** - Детали измерения

#### Компоненты (Components)

##### Диалоги (Components/Dialog)
- **AddExerciseDialog.razor** - Добавление упражнения
- **EditExerciseDialog.razor** - Редактирование упражнения
- **EditWorkoutDialog.razor** - Редактирование тренировки
- **EditWorkoutPlanDialog.razor** - Редактирование плана тренировок
- **EditWorkoutExerciseNotesDialog.razor** - Редактирование заметок к упражнению
- **SelectExercisesDialog.razor** - Выбор упражнений
- **SelectWorkoutDialog.razor** - Выбор тренировки
- **SelectWorkoutPlanDialog.razor** - Выбор плана тренировок
- **AddWorkoutsToPlanDialog.razor** - Добавление тренировок в план
- **ReplaceExerciseDialog.razor** - Замена упражнения
- **ConfirmDialog.razor** - Диалог подтверждения

##### Layout компоненты (Components/Layout)
- **MainLayout.razor** - Главный layout
- **NavMenu.razor** - Навигационное меню
- **WorkoutCard.razor** - Карточка тренировки
- **AddSetForm.razor** - Форма добавления подхода
- **ImageUpload.razor** - Компонент загрузки изображений
- **MuscleGroupsFilter.razor** - Фильтр по группам мышц

#### Сервисы (Services)
- **IUserService / UserService** - Работа с пользователями
- **IExerciseService / ExerciseService** - Работа с упражнениями
- **IWorkoutService / WorkoutService** - Работа с тренировками
- **IWorkoutPlanService / WorkoutPlanService** - Работа с планами тренировок
- **IWorkoutExerciseService / WorkoutExerciseService** - Работа с упражнениями в тренировках
- **ISetService / SetService** - Работа с подходами
- **IMeasurementsHistoryService / MeasurementsHistoryService** - Работа с измерениями
- **IWorkoutHistoryService / WorkoutHistoryService** - Работа с историей тренировок
- **IWorkoutHistorySetService / WorkoutHistorySetService** - Работа с подходами в истории
- **IFileUploadService / FileUploadService** - Загрузка файлов
- **IActiveWorkoutService / ActiveWorkoutService** - Управление активной тренировкой
- **ITelegramWebAppService / TelegramWebAppService** - Работа с Telegram WebApp API
- **ITelegramWebAppStateService / TelegramWebAppStateService** - Управление состоянием Telegram WebApp
- **ITelegramBackButtonService / TelegramBackButtonService** - Управление кнопкой "Назад" в Telegram
- **IAppInitializationService / AppInitializationService** - Инициализация приложения

#### Расширения (Extensions)
- **HttpClientExtensions** - Расширения для HTTP клиента
  - Добавление JWT токена в заголовки запросов
  - Обработка ошибок аутентификации

#### JavaScript интеграция (wwwroot/js)
- **telegramWebApp.js** - Интеграция с Telegram WebApp API
  - Инициализация Telegram WebApp
  - Работа с кнопкой "Назад"
  - Управление состоянием приложения

- **workoutDragDrop.js** - Функциональность drag & drop для тренировок

#### Стили (wwwroot/css)
- **mobile.css** - Стили для мобильных устройств
- **workout-common.css** - Общие стили для тренировок

#### Конфигурация
- **appsettings.json** - Настройки приложения
  - `ApiBaseUrl` - Базовый URL API
  - Настройки Telegram WebApp

#### Особенности
- Использует MudBlazor для UI компонентов
- Полная интеграция с Telegram WebApp API
- Поддержка drag & drop для упорядочивания упражнений
- Адаптивный дизайн для мобильных устройств
- JWT аутентификация через HTTP заголовки

---

## База данных

### PostgreSQL

**Версия:** PostgreSQL 16 (Alpine)

**Структура:**
- Все таблицы создаются через EF Core миграции
- Миграции находятся в проекте `SetRep.Infrastructure/Migrations/`
- Миграции применяются автоматически при старте API

**Основные таблицы:**
- `Users` - Пользователи
- `Exercises` - Упражнения
- `Workouts` - Тренировки
- `WorkoutExercises` - Связь тренировок и упражнений
- `Sets` - Подходы
- `WorkoutPlans` - Планы тренировок
- `WorkoutHistory` - История тренировок
- `WorkoutHistorySets` - Подходы в истории
- `MeasurementsHistory` - История измерений

**Индексы:**
- `Users.TelegramId` - Уникальный индекс
- `Users.Username` - Уникальный индекс

**Связи:**
- Каскадное удаление для связанных сущностей
- Внешние ключи настроены через конфигурации EF Core

---

## Аутентификация и авторизация

### Процесс аутентификации

1. **Пользователь открывает MiniApp в Telegram**
   - Telegram передает данные `tgWebAppData` в приложение

2. **Валидация данных Telegram WebApp**
   - Клиент отправляет данные на `/api/telegramwebapp/validate`
   - Сервер проверяет подпись данных с использованием токена бота

3. **Получение JWT токена**
   - Клиент отправляет валидированные данные на `/api/auth/login`
   - Сервер:
     - Проверяет существование пользователя по `TelegramId`
     - Создает пользователя, если не существует
     - Генерирует JWT токен с данными пользователя

4. **Использование токена**
   - JWT токен сохраняется в клиентском приложении
   - Все последующие запросы к API включают токен в заголовке `Authorization: Bearer {token}`
   - API валидирует токен и извлекает данные пользователя

### JWT токен

**Содержимое токена:**
- `userId` (Guid) - ID пользователя в базе данных
- `telegramId` (long) - ID пользователя в Telegram
- `username` (string) - Имя пользователя
- `jti` (Guid) - Уникальный идентификатор токена
- `iat` (long) - Время выдачи токена

**Время жизни:** 24 часа (настраивается через `Jwt__ExpirationMinutes`)

**Алгоритм:** HMAC SHA256

---

## Хранилище файлов

### Yandex Object Storage

**Назначение:** Хранение изображений упражнений и других медиа-файлов.

**Интеграция:**
- Используется S3-совместимое API через AWSSDK.S3
- Сервис `YandexObjectStorageService` в проекте `SetRep.API`

**Функциональность:**
- Загрузка изображений
- Удаление изображений
- Обработка изображений перед загрузкой (ImageSharp, Magick.NET)

**Настройки:**
- `YANDEX_OBJECT_STORAGE_ACCESS_KEY` - Access key
- `YANDEX_OBJECT_STORAGE_SECRET_KEY` - Secret key
- `YANDEX_OBJECT_STORAGE_ENDPOINT` - Endpoint (по умолчанию: `https://storage.yandexcloud.net`)
- `YANDEX_OBJECT_STORAGE_BUCKET_NAME` - Имя бакета
- `YANDEX_OBJECT_STORAGE_SERVICE_URL` - URL сервиса

**Ограничения:**
- Максимальный размер файла настраивается через `FileUpload__MaxFileSizeBytes`
- Максимальное количество загрузок на пользователя через `FileUpload__MaxUploadsPerUser`

---

## Docker и развертывание

### Docker Compose

**Файл:** `docker-compose.yml`

**Сервисы:**
1. **postgres** - PostgreSQL 16 база данных
2. **api** - SetRep.API
3. **bot** - SetRep.Bot
4. **miniapp** - SetRep.MiniApp

**Сеть:** `setrep-network` (bridge)

**Volumes:**
- `postgres_data` - Данные PostgreSQL

### Переменные окружения

Все переменные окружения описаны в файле `env.example`.

**Обязательные переменные:**
- `BOT_TOKEN` - Токен Telegram бота
- `JWT_SECRET_KEY` - Секретный ключ для JWT (минимум 32 символа)
- `BOT_WEBHOOK_URL` - URL для webhook бота
- `YANDEX_OBJECT_STORAGE_ACCESS_KEY` - Access key для Yandex Object Storage
- `YANDEX_OBJECT_STORAGE_SECRET_KEY` - Secret key для Yandex Object Storage
- `YANDEX_OBJECT_STORAGE_BUCKET_NAME` - Имя бакета

**Порты:**
- `API_HTTP_PORT` - Порт API (по умолчанию: 5181)
- `BOT_HTTP_PORT` - Порт бота (по умолчанию: 5083)
- `MINIAPP_HTTP_PORT` - Порт MiniApp (по умолчанию: 5096)
- `POSTGRES_PORT` - Порт PostgreSQL (по умолчанию: 5432)

### Запуск

1. Скопировать `env.example` в `.env` и заполнить переменные
2. Запустить: `docker-compose up -d`
3. Установить webhook для бота: `GET http://localhost:5083/bot/setWebhook`

---

## Разработка

### Требования

- .NET 9.0 SDK или выше
- .NET 10.0 SDK для проектов API и Infrastructure
- PostgreSQL 16 (или Docker)
- Docker и Docker Compose (для контейнеризации)

### Локальная разработка

1. **Настройка базы данных:**
   - Запустить PostgreSQL через Docker: `docker-compose up -d postgres`
   - Или использовать локальный PostgreSQL

2. **Настройка переменных окружения:**
   - Использовать User Secrets для разработки
   - Или создать `appsettings.Development.json`

3. **Применение миграций:**
   - Миграции применяются автоматически при старте API
   - Или вручную: `dotnet ef database update --project SetRep.Infrastructure --startup-project SetRep.API`

4. **Запуск проектов:**
   - API: `dotnet run --project SetRep.API`
   - Bot: `dotnet run --project SetRep.Bot`
   - MiniApp: `dotnet run --project SetRep.MiniApp/SetRep.MiniApp`

### Структура папок

```
SetRep/
├── SetRep.API/              # REST API
├── SetRep.Bot/              # Telegram бот
├── SetRep.Core/             # Доменная модель
├── SetRep.Infrastructure/   # Доступ к данным
├── SetRep.MiniApp/          # Blazor приложение
│   ├── SetRep.MiniApp/      # Серверный хост
│   └── SetRep.MiniApp.Client/ # WebAssembly клиент
├── SetRep.Shared/           # Общие DTO
├── docker-compose.yml       # Docker Compose конфигурация
├── env.example              # Пример переменных окружения
└── Documentation.md        # Эта документация
```

---

## API Endpoints

### Аутентификация
- `POST /api/auth/login` - Вход через Telegram WebApp данные

### Пользователи
- `GET /api/users/{id}` - Получить пользователя
- `GET /api/users/telegram/{telegramId}` - Получить пользователя по Telegram ID
- `POST /api/users` - Создать пользователя
- `PUT /api/users/{id}` - Обновить пользователя

### Упражнения
- `GET /api/exercises` - Список упражнений (с фильтрами)
- `GET /api/exercises/{id}` - Получить упражнение
- `POST /api/exercises` - Создать упражнение
- `PUT /api/exercises/{id}` - Обновить упражнение
- `DELETE /api/exercises/{id}` - Удалить упражнение

### Тренировки
- `GET /api/workouts` - Список тренировок пользователя
- `GET /api/workouts/{id}` - Получить тренировку
- `POST /api/workouts` - Создать тренировку
- `PUT /api/workouts/{id}` - Обновить тренировку
- `DELETE /api/workouts/{id}` - Удалить тренировку

### Планы тренировок
- `GET /api/workoutplans` - Список планов пользователя
- `GET /api/workoutplans/{id}` - Получить план
- `POST /api/workoutplans` - Создать план
- `PUT /api/workoutplans/{id}` - Обновить план
- `DELETE /api/workoutplans/{id}` - Удалить план

### Подходы
- `GET /api/sets` - Список подходов
- `GET /api/sets/{id}` - Получить подход
- `POST /api/sets` - Создать подход
- `PUT /api/sets/{id}` - Обновить подход
- `DELETE /api/sets/{id}` - Удалить подход

### История тренировок
- `GET /api/workouthistory` - Список истории тренировок
- `GET /api/workouthistory/{id}` - Получить запись истории
- `POST /api/workouthistory` - Создать запись истории
- `PUT /api/workouthistory/{id}` - Обновить запись истории
- `DELETE /api/workouthistory/{id}` - Удалить запись истории

### Измерения
- `GET /api/measurementshistory` - Список измерений
- `GET /api/measurementshistory/{id}` - Получить измерение
- `POST /api/measurementshistory` - Создать измерение
- `PUT /api/measurementshistory/{id}` - Обновить измерение
- `DELETE /api/measurementshistory/{id}` - Удалить измерение

### Загрузка файлов
- `POST /api/fileupload/image` - Загрузить изображение
- `DELETE /api/fileupload/image` - Удалить изображение

### Telegram WebApp
- `POST /api/telegramwebapp/validate` - Валидировать данные Telegram WebApp

**Примечание:** Все endpoints (кроме `/api/auth/login` и `/api/telegramwebapp/validate`) требуют JWT аутентификации.

---

## Особенности реализации

### Автоматическое завершение тренировок

Фоновый сервис `WorkoutAutoCompletionService` автоматически завершает незавершенные тренировки через определенный период времени. Сервис запускается при старте API и периодически проверяет активные тренировки.

### Drag & Drop для упражнений

В MiniApp реализована функциональность drag & drop для изменения порядка упражнений в тренировке. Используется JavaScript библиотека для обработки событий перетаскивания.

### Обработка изображений

Перед загрузкой изображения обрабатываются:
- Проверка размера файла
- Конвертация формата (при необходимости)
- Оптимизация размера

Используются библиотеки:
- SixLabors.ImageSharp (в API)
- Magick.NET (в MiniApp)

### Интеграция с Telegram WebApp

Полная интеграция с Telegram WebApp API:
- Инициализация приложения
- Управление кнопкой "Назад"
- Управление состоянием приложения (закрытие, разворачивание)
- Работа с темой Telegram

---

## Безопасность

### JWT токены
- Используется HMAC SHA256 для подписи
- Валидация времени жизни токена
- Валидация издателя и аудитории

### Telegram WebApp валидация
- Проверка подписи данных с использованием токена бота
- Защита от подделки данных

### CORS
- Настроен только для разрешенных источников
- Поддержка credentials для передачи токенов

### Загрузка файлов
- Ограничение размера файлов
- Ограничение количества загрузок на пользователя
- Валидация типов файлов

---

## Логирование

Все проекты используют стандартное логирование .NET (`ILogger`):
- API логирует операции с базой данных, ошибки аутентификации
- Bot логирует все входящие сообщения и ошибки
- MiniApp логирует операции инициализации и ошибки

---

## Тестирование

В настоящее время проект не содержит автоматических тестов. Рекомендуется добавить:
- Unit тесты для бизнес-логики
- Integration тесты для API
- E2E тесты для MiniApp

---

## Дальнейшее развитие

Возможные направления развития:
- Добавление статистики и аналитики тренировок
- Экспорт данных в различные форматы
- Социальные функции (обмен тренировками)
- Интеграция с фитнес-трекерами
- Мобильное приложение (iOS/Android)
- Push-уведомления о тренировках
- Планировщик тренировок с календарем

---

## Контакты и поддержка

Для вопросов и предложений создавайте issues в репозитории проекта.

---

**Последнее обновление:** 2026-02-04
