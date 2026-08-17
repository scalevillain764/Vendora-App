# Vendora

Vendora --- backend интернет-маркетплейса на ASP.NET Core. Проект
построен вокруг REST API и разделён на несколько логических слоёв:
Presentation, Application, Domain и Infrastructure.

Проект поддерживает пользователей, магазины, товары, корзину, избранное,
заказы, оплату, отзывы, вопросы, поиск, статистику продавца и загрузку
изображений в S3-совместимое хранилище Garage.

------------------------------------------------------------------------

## О проекте

Vendora позволяет пользователю выступать одновременно в роли покупателя
и продавца.

Основной сценарий:

``` text
Регистрация
    ↓
Создание магазина
    ↓
Создание товара
    ↓
Поиск / избранное
    ↓
Корзина
    ↓
Заказ
    ↓
Оплата
    ↓
Статистика продавца
```

Для работы с изображениями используется отдельный S3-совместимый storage
--- Garage.

Архитектура взаимодействия с изображениями:

``` text
Frontend
    ↓
Image API
    ↓
Garage / S3
    ↓
URL изображения
    ↓
Product / Store / Review / Question
```

------------------------------------------------------------------------

## Основные возможности

### Пользователь

-   регистрация и авторизация;
-   JWT access token;
-   refresh token в HttpOnly cookie;
-   изменение профиля;
-   изменение логина и пароля;
-   загрузка аватара;
-   удаление аккаунта.

### Магазин

-   создание магазина;
-   изменение названия и описания;
-   загрузка аватара;
-   удаление магазина;
-   получение публичной информации о магазине.

### Товары

-   создание и удаление товаров;
-   изменение названия, категории, цены, количества и описания;
-   загрузка preview;
-   добавление и удаление дополнительных изображений;
-   получение товаров магазина;
-   получение отдельного товара.

### Покупатель

-   поиск и фильтрация товаров;
-   избранное;
-   корзина;
-   изменение количества товаров в корзине;
-   создание заказа;
-   просмотр своих заказов.

### Продавец

-   статистика продаж товара;
-   ответы на отзывы;
-   ответы на вопросы покупателей.

### Дополнительно

-   отзывы;
-   вопросы к товарам;
-   оплата с баланса;
-   интеграция с YooKassa;
-   получение курсов валют;
-   S3-хранилище через Garage.

------------------------------------------------------------------------

## Технологии

  Технология              Назначение
  ----------------------- -----------------------------------
  C#                      Основной язык
  .NET 10                 Backend platform
  ASP.NET Core Web API    REST API
  Entity Framework Core   Работа с БД
  PostgreSQL 16           Основная база данных
  JWT                     Аутентификация
  FluentValidation        Валидация DTO
  Swagger / OpenAPI       Документация API
  AWS SDK for S3          Работа с S3 API
  Garage                  S3-совместимое файловое хранилище
  YooKassa                Онлайн-оплата
  Docker                  Контейнеризация
  Docker Compose          Локальный запуск окружения
  ULID                    Идентификаторы сущностей

------------------------------------------------------------------------

## Архитектура

Проект разделён на четыре основных слоя:

``` text
Presentation
    ↓
Application
    ↓
Domain

Infrastructure
    ↑
Application
```

### Presentation

Содержит:

-   Controllers;
-   middleware;
-   HTTP-обвязку;
-   обработку результатов сервисов.

### Application

Содержит:

-   DTO;
-   interfaces;
-   services;
-   validators;
-   бизнес-логику приложения.

### Domain

Содержит:

-   сущности;
-   enum;
-   доменные модели;
-   типы ошибок.

### Infrastructure

Содержит:

-   `AppDbContext`;
-   EF Core configuration;
-   migrations;
-   интеграцию с внешними сервисами.

------------------------------------------------------------------------

## Структура проекта

``` text
Vendora/
├── Application/
│   ├── DTO/
│   ├── Interfaces/
│   ├── Services/
│   └── Validators/
│
├── Domain/
│   ├── Products/
│   ├── Stores/
│   ├── Users/
│   ├── Orders/
│   └── ...
│
├── Infrastructure/
│   └── AppDbContexts/
│
├── Presentation/
│   ├── Controllers/
│   └── ExceptionMiddlewares/
│
├── Migrations/
├── Dockerfile
├── docker-compose.yml
├── garage.toml
├── appsettings.json
├── .env.example
└── Vendora.csproj
```

------------------------------------------------------------------------

# Запуск проекта

Для запуска через Docker нужны:

-   Docker;
-   Docker Compose;
-   Git.

Клонировать репозиторий:

``` bash
git clone <repository-url>
cd Vendora
```

------------------------------------------------------------------------

## 1. Создать `.env`

В проекте уже есть пример конфигурации:

``` text
.env.example
```

Скопировать его:

``` bash
cp .env.example .env
```

В Windows PowerShell:

``` powershell
Copy-Item .env.example .env
```

После этого заполнить значения в `.env`.

Минимально необходимая конфигурация:

``` env
SECRET_KEY=your_secret_key

POSTGRES_PORT=5432
POSTGRES_DATABASE=VendoraAppDB
POSTGRES_USERNAME=postgres
POSTGRES_PASSWORD=your_password

GARAGE_API_KEY=your_garage_api_key
GARAGE_SECRET_KEY=your_garage_secret_key
GARAGE_REGION=garage
GARAGE_SERVICE_URL=http://garage:3900
GARAGE_BUCKET_NAME=vendora
GARAGE_BASE_URL=http://localhost:3900

YOOKASSA_SHOP_ID=your_shop_id
YOOKASSA_API_KEY=your_api_key
YOOKASSA_BACK_URL=your_back_url

EXCHANGE_RATES_BASE_URL=your_base_url
EXCHANGE_RATES_API_KEY=your_api_key
EXCHANGE_RATES_ENDPOINT_SUFFIX=your_suffix
```

Не добавляйте `.env` в Git. Секреты, API keys и пароли не должны
попадать в репозиторий.

------------------------------------------------------------------------

## 2. Запустить Docker Compose

Из корня проекта:

``` bash
docker compose up --build
```

Для запуска в фоне:

``` bash
docker compose up --build -d
```

Docker Compose поднимет три контейнера:

``` text
web-api
    ↓
PostgreSQL

web-api
    ↓
Garage
```

Фактически сервисы:

``` text
┌───────────────────────────────┐
│           Vendora             │
│          ASP.NET Core         │
│          port 5000            │
└───────────────┬───────────────┘
                │
       ┌────────┴────────┐
       ↓                 ↓
┌──────────────┐   ┌──────────────┐
│ PostgreSQL   │   │    Garage    │
│    :7000     │   │    :3900     │
└──────────────┘   └──────────────┘
```

------------------------------------------------------------------------

## 3. Проверить API

После запуска API доступен по адресу:

``` text
http://localhost:5000
```

Swagger:

``` text
http://localhost:5000/swagger
```

Swagger позволяет посмотреть все доступные endpoint'ы и отправлять
запросы напрямую.

Для защищённых endpoint'ов необходимо получить JWT после
регистрации/логина и указать:

``` text
Bearer <access_token>
```

в Swagger через кнопку `Authorize`.

------------------------------------------------------------------------

## PostgreSQL

Внутри Docker-сети API подключается к PostgreSQL по имени сервиса:

``` text
Host=db
```

Это важно.

Из контейнера `web-api`:

``` text
db:5432
```

Из системы хоста:

``` text
localhost:7000
```

В `docker-compose.yml` используется:

``` yaml
ports:
  - "7000:5432"
```

То есть:

``` text
localhost:7000
      ↓
Docker PostgreSQL:5432
```

Данные PostgreSQL сохраняются в Docker volume:

``` text
postgres_data
```

Поэтому удаление контейнера не удаляет базу данных.

------------------------------------------------------------------------

## Миграции EF Core

Миграции находятся в:

``` text
Migrations/
```

В текущей конфигурации миграции не применяются автоматически при старте
приложения.

Если структура базы ещё не создана, миграции можно применить командой:

``` bash
dotnet ef database update
```

Если `dotnet ef` не установлен:

``` bash
dotnet tool install --global dotnet-ef
```

При работе с Docker важно, откуда выполняется команда и какой connection
string используется.

Для локального запуска приложения connection string может использовать:

``` text
Host=localhost
```

Для приложения внутри Docker Compose используется:

``` text
Host=db
```

Именно поэтому connection string для контейнера задаётся через:

``` text
ConnectionStrings__DefaultConnection
```

в `docker-compose.yml`.

------------------------------------------------------------------------

## Garage / S3

Garage используется как S3-совместимое хранилище изображений.

Docker Compose запускает Garage и открывает:

    Port Назначение
  ------ ------------
    3900 S3 API
    3901 RPC
    3903 Admin API

Приложение обращается к Garage через:

``` text
http://garage:3900
```

поскольку `web-api` и `garage` находятся в одной Docker network.

С хоста S3 API доступен через:

``` text
http://localhost:3900
```

Файлы используются для:

-   аватаров пользователей;
-   аватаров магазинов;
-   preview товаров;
-   дополнительных изображений товаров;
-   фотографий отзывов;
-   фотографий вопросов.

------------------------------------------------------------------------

## Работа с изображениями

Есть два основных подхода.

### Загрузка через Image API

Используется, когда основной DTO принимает URL.

Например, создание товара:

``` text
Frontend
    ↓
POST /api/Image
    ↓
previewUrl

POST /api/Image/images
    ↓
pictures[]

POST /api/Product
    ↓
ProductCreationDTO
```

То есть файл сначала загружается в S3, затем полученные URL передаются в
DTO создания товара.

То же самое используется для фотографий отзывов и вопросов.

### Прямая загрузка

Некоторые endpoint'ы сами принимают `IFormFile`.

Например:

``` text
PATCH /api/User/profile_picture
PATCH /api/Store/profile_picture
PATCH /api/Product/preview/{ProductId}
```

В этом случае отдельно вызывать `/api/Image` не требуется.

------------------------------------------------------------------------

## API

Основные группы endpoint'ов:

``` text
/api/Auth
/api/User
/api/Store
/api/Image
/api/Product
/api/Search
/api/Favourite
/api/Cart
/api/Order
/api/Payment
/api/ProductStatistics
/api/ProductReview
/api/UserQuestion
/api/ExchangeRates
```

Полная документация API находится в отдельном документе:

``` text
Vendora API Documentation
```

Swagger также доступен после запуска приложения:

``` text
http://localhost:5000/swagger
```

------------------------------------------------------------------------

## Авторизация

Большинство endpoint'ов требуют JWT.

После успешного login клиент получает:

``` text
accessToken
```

и refresh token в HttpOnly cookie.

Для защищённого запроса:

``` http
Authorization: Bearer <access_token>
```

JWT используется ASP.NET Core Authentication Middleware.

------------------------------------------------------------------------

## Заказы и оплата

Основной flow покупки:

``` text
GET /api/Cart
    ↓
POST /api/Order
    ↓
Pending Order
    ↓
┌──────────────────────┐
│                      │
▼                      ▼
Balance             YooKassa
│                      │
▼                      ▼
Payment             Webhook
└──────────┬───────────┘
           ↓
PaymentCompleted
```

При создании заказа backend:

-   проверяет корзину;
-   проверяет наличие товара;
-   создаёт заказ;
-   создаёт позиции заказа;
-   уменьшает количество товаров;
-   очищает корзину.

------------------------------------------------------------------------

## Статистика продавца

Статистика отделена от обычного товара.

Для страницы товара:

``` text
GET /api/Product/{ProductId}
```

Для статистики продавца:

``` text
GET /api/ProductStatistics/{StoreId}/{ProductId}
```

Это позволяет не отдавать продавцу внутреннюю статистику обычному
покупателю вместе с базовой информацией о товаре.

------------------------------------------------------------------------

## Обработка ошибок

В приложении используется `Result` pattern и middleware обработки
исключений.

Типовые ошибки API:

  Тип              HTTP Значение
  -------------- ------ -----------------------
  Validation        400 Некорректные данные
  Unauthorized      401 Требуется авторизация
  Forbidden         403 Недостаточно прав
  NotFound          404 Сущность не найдена
  Conflict          409 Конфликт состояния
  Exception         500 Внутренняя ошибка

------------------------------------------------------------------------

## Локальная разработка без Docker

Для запуска без Docker необходимо установить:

-   .NET 10 SDK;
-   PostgreSQL;
-   Garage или другое S3-совместимое хранилище.

После настройки переменных окружения:

``` bash
dotnet restore
dotnet build
dotnet run
```

При таком запуске PostgreSQL должен быть доступен через `localhost`.

------------------------------------------------------------------------

## Остановка окружения

Остановить контейнеры:

``` bash
docker compose down
```

Остановить контейнеры и удалить volumes:

``` bash
docker compose down -v
```

Второй вариант удалит сохранённые данные PostgreSQL и Garage.
Используйте его только если данные больше не нужны.

------------------------------------------------------------------------

## Текущий статус

Проект находится в активной разработке.

Уже реализованы основные backend-модули:

-   authentication;
-   users;
-   stores;
-   products;
-   search;
-   favourites;
-   cart;
-   orders;
-   payments;
-   reviews;
-   questions;
-   product statistics;
-   exchange rates;
-   S3-compatible image storage;
-   Docker environment.

В планах дальнейшего развития:

-   Redis;
-   unit tests;
-   integration tests;
-   улучшение логирования;
-   дальнейшее развитие frontend-части;
-   дополнительные механизмы кеширования и оптимизации.

------------------------------------------------------------------------

## Автор

Backend разработан на C# / ASP.NET Core.

Проект создан как практический marketplace backend с упором на REST API,
работу с PostgreSQL, внешними сервисами, файловым хранилищем и
контейнеризацией.
