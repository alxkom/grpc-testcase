
## Описание

Задание заключается в выносе части функционала приложения **TaskManager** в отдельный проект **TaskManagerProvider**, который будет предоставлять **GRPC** интерфейс для работы с данными, вместо реализации **DataService**.

Схема взаимодействия с данными в текущей реализации:

```plantuml
scale 0.8
actor  User
box TaskManager
    participant TaskManager
    control DataService
end box

User -> TaskManager: Запрос данных
TaskManager -> DataService: Запрос данных
DataService -> TaskManager: Данные
TaskManager -> User: Данные
```

Требуемая схема взаимодействия с данными:

```plantuml
scale 0.8
actor  User
box TaskManager
    participant TaskManager
    control DataService
end box

box TaskManagerProvider
participant TaskManagerProvider
end box

User -> TaskManager: Запрос данных
TaskManager -> DataService: Запрос данных
DataService ----> TaskManagerProvider: Запрос данных по GRPC
TaskManagerProvider ---> DataService: Данные по GRPC
DataService --> TaskManager: Данные
TaskManager -> User: Данные
```

## Задание

Создайте в решении новый проект **TaskManagerProvider** на C#, который будет соответствовать следующим требованиям:

- Фреймворк .NET8
- Приложение командной строки
- Приложение предоставляет **GRPC** интерфейс, аналогичный **IDataService** из **TaskManager** и его моделям
- Предоставляет proto файл с описанием **GRPC** сервиса

Для этого в проекте **TaskManagerProvider**:

- создайте proto файл, описывающий **GRPC** сервис и модели, аналогично **IDataService**
- добавьте server proto описание в проект
- реализуйте **GRPC** сервис, аналогично реализации **DataService** (in memory data storage)

В проекте **TaskManager**:

- добавьте client proto описание в проект
- обеспечьте конвертацию моделей между **GRPC** и моделями из **IDataService**
- измените реализацию **DataService** так, чтобы использовался сгенерированный **GRPC** клиент для работы с данными
- добавьте в проект простую конфигурацию с описанием **GRPC** endpoint

Дополнительно:

- добавьте методы удаления записей в **IDataService**, **GRPC** описание и реализацию в **TaskManagerProvider**
- добавьте функционал удаления записей в веб интерфейс **TaskManager**
- удаление пользователя должно приводить к каскадному удалению всех задач пользователя
  
При запуске обеих проектов веб интерфейс **TaskManager** должен работать аналогично текущему, но данные должны обрабатываться по **GRPC** в **TaskManagerProvider**.

