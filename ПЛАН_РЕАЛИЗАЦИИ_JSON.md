# План реализации работы с JSON-файлами

## 📋 Техническое задание
1. Выполнить сохранение списка объектов в JSON-документ
2. Выполнить чтение из JSON-документа в список объектов
3. Сохранить проект в отдельной ветке Json репозитория

---

## 🎯 Цель
Реализовать обмен информацией между списком объектов (`BankAccount`, `Client`) и JSON-файлом для сохранения данных между сеансами работы приложения.

---

## 📦 Что нужно сохранять

### 1. Банковские счета (`BankAccount`)
- Номер счета (`AccountNumber`)
- Дата открытия (`OpeningDate`)
- Владелец (`Owner` - ссылка на `Client`)
- Баланс (`Balance`)
- Срок вклада (`DepositTermDays`)
- Статус (`Status`)

### 2. Клиенты (`Client`)
- ФИО (`FullName`)
- Паспортные данные (`PassportData`)
- Дата рождения (`DateOfBirth`)

---

## 🏗️ Архитектура решения

### Структура файлов
```
SimpleBank/
├── Models/
│   ├── Client.cs (существует)
│   ├── BankAccount.cs (существует)
│   └── BankData.cs (НОВЫЙ - контейнер для всех данных)
├── Services/
│   └── JsonDataService.cs (НОВЫЙ - сервис для работы с JSON)
└── Data/
    └── bank_data.json (НОВЫЙ - файл для хранения данных)
```

---

## 📝 Этап 1: Подготовка моделей для сериализации

### Задача 1.1: Создать контейнер данных
**Файл:** `Models/BankData.cs`

**Содержание:**
- Класс `BankData` с двумя свойствами:
  - `List<Client> Clients` - список клиентов
  - `List<BankAccount> Accounts` - список счетов

**Цель:** Объединить все данные в один объект для удобной сериализации

---

### Задача 1.2: Подготовить модели к сериализации
**Файлы:** `Models/Client.cs`, `Models/BankAccount.cs`, `Models/BankData.cs`

**Требования для `DataContractJsonSerializer`:**
1. Для класса задать атрибут `[DataContract]` или `[Serializable]`
2. Для всех свойств, которые нужно сериализовать, задать атрибут `[DataMember]`

**Изменения:**
- Добавить `using System.Runtime.Serialization;` в начало файлов
- Добавить атрибут `[DataContract]` перед каждым классом (`Client`, `BankAccount`, `BankData`)
- Добавить атрибут `[DataMember]` перед каждым свойством, которое нужно сериализовать
- Для `BankAccount.Owner` - при сериализации сохранять паспортные данные клиента, при десериализации - восстанавливать ссылку по паспорту

**Пример:**
```csharp
[DataContract]
public class Client
{
    [DataMember]
    public string FullName { get; set; }
    
    [DataMember]
    public string PassportData { get; set; }
    
    [DataMember]
    public DateTime DateOfBirth { get; set; }
}
```

---

## 📝 Этап 2: Подключение библиотеки и пространств имен

### Задача 2.1: Добавить ссылку на библиотеку
**Действия:**
1. В Visual Studio в Solution Explorer найти папку "References" (Ссылки)
2. Правой кнопкой мыши по "References" → "Add Reference..." (Добавить ссылку...)
3. В окне "Reference Manager" (Менеджер ссылок) выбрать "Assemblies" (Сборки) → "Framework"
4. Найти и отметить `System.Runtime.Serialization`
5. Нажать "OK"

**Результат:** В References появится `System.Runtime.Serialization`

---

### Задача 2.2: Подключить пространства имен
**Файл:** `Services/JsonDataService.cs`

**Необходимые using:**
```csharp
using System.Runtime.Serialization.Json;  // Для DataContractJsonSerializer
using System.Runtime.Serialization;       // Для атрибутов [DataContract], [DataMember]
using System.IO;                          // Для FileStream
```

---

## 📝 Этап 3: Создание сервиса для работы с JSON

### Задача 3.1: Создать класс `JsonDataService`
**Файл:** `Services/JsonDataService.cs`

**Методы:**
1. `SaveToJson(BankData data, string filePath)` - сохранение данных в JSON
2. `LoadFromJson(string filePath)` - загрузка данных из JSON
3. `GetDataFilePath()` - получение пути к файлу данных

**Технологии:**
- `DataContractJsonSerializer` (класс из `System.Runtime.Serialization.Json`)
- `FileStream` (класс из `System.IO`) для работы с файлами
- `FileMode.Create` - для создания/перезаписи файла
- `FileMode.Open` - для чтения существующего файла

---

### Задача 3.2: Реализация метода сохранения
**Пример кода:**
```csharp
public void SaveToJson(BankData data, string filePath)
{
    // Создание объекта DataContractJsonSerializer
    DataContractJsonSerializer jsonSerializer = 
        new DataContractJsonSerializer(typeof(BankData));
    
    // Создание файлового потока для записи
    FileStream fileStream = new FileStream(filePath, FileMode.Create);
    
    // Сериализация объекта в поток
    jsonSerializer.WriteObject(fileStream, data);
    
    // Закрытие потока
    fileStream.Close();
}
```

**Примечание:** Используется `FileMode.Create` - файл создается или перезаписывается, если существует.

---

### Задача 3.3: Реализация метода загрузки
**Пример кода:**
```csharp
public BankData LoadFromJson(string filePath)
{
    // Создание объекта DataContractJsonSerializer
    DataContractJsonSerializer jsonSerializer = 
        new DataContractJsonSerializer(typeof(BankData));
    
    // Создание файлового потока для чтения
    FileStream fileStream = new FileStream(filePath, FileMode.Open);
    
    // Десериализация из потока (требуется преобразование типа)
    BankData data = (BankData)jsonSerializer.ReadObject(fileStream);
    
    // Закрытие потока
    fileStream.Close();
    
    return data;
}
```

**Примечание:** 
- Используется `FileMode.Open` - открывается существующий файл
- Метод `ReadObject` возвращает `object`, требуется явное приведение типа `(BankData)`

---

### Задача 3.4: Обработка ошибок
- Проверка существования файла перед чтением
- Обработка `FileNotFoundException` при чтении
- Обработка `SerializationException` при десериализации
- Использование `try-catch` блоков
- Создание файла по умолчанию, если его нет при первом запуске

---

## 📝 Этап 4: Интеграция с существующим кодом

### Задача 4.1: Модификация `BankAccountsWindow.xaml.cs`

**Изменения:**
1. Добавить поле `JsonDataService _dataService`
2. Заменить локальные переменные `_account1`, `_account2` на `List<BankAccount> _accounts`
3. Заменить `_clients` на загрузку из JSON
4. При создании/изменении счета - сохранять в JSON
5. При закрытии окна - сохранять данные

**Методы для добавления:**
- `LoadData()` - загрузка данных при открытии окна
- `SaveData()` - сохранение данных
- `AddAccount(BankAccount account)` - добавление счета в список
- `UpdateAccount(BankAccount account)` - обновление счета

---

### Задача 4.2: Автоматическое сохранение
**Варианты:**
1. Сохранение при каждом изменении (создание, пополнение, снятие, перевод)
2. Сохранение при закрытии окна
3. Сохранение по кнопке "Сохранить"

**Рекомендация:** Сохранение при каждом изменении + при закрытии окна

---

## 📝 Этап 5: Работа с вложенными объектами

### Задача 5.1: Сериализация `BankAccount.Owner`

**Проблема:** `BankAccount` содержит ссылку на `Client`. `DataContractJsonSerializer` автоматически сериализует вложенные объекты, но при десериализации может возникнуть проблема с восстановлением ссылок между объектами.

**Решение:**
1. При сериализации `BankAccount` сохранять паспортные данные владельца в отдельном поле `OwnerPassportData` (не сериализовать сам объект `Owner`)
2. При десериализации восстанавливать ссылку на `Client` по паспортным данным из списка клиентов

**Реализация:**
- В `BankAccount` добавить свойство `[DataMember] public string OwnerPassportData { get; set; }` для сериализации
- Свойство `Owner` НЕ помечать `[DataMember]` (не сериализуется)
- При сохранении: `account.OwnerPassportData = account.Owner?.PassportData;`
- При загрузке: `account.Owner = clients.FirstOrDefault(c => c.PassportData == account.OwnerPassportData);`

---

## 📝 Этап 6: Структура JSON-файла

### Пример структуры (результат работы DataContractJsonSerializer):
```json
[
  {
    "Clients": [
      {
        "FullName": "Кузьмин Олег Иванович",
        "PassportData": "1234 567890",
        "DateOfBirth": "\/Date(1234567890000+0300)\/"
      }
    ],
    "Accounts": [
      {
        "AccountNumber": "12345678901234567890",
        "OpeningDate": "\/Date(1705276800000+0300)\/",
        "OwnerPassportData": "1234 567890",
        "Balance": 10000.50,
        "DepositTermDays": 365,
        "Status": 0
      }
    ]
  }
]
```

**Особенности DataContractJsonSerializer:**
- **Формат даты:** Используется формат `"/Date(миллисекунды+часовой_пояс)/"` вместо ISO 8601
- **Ключи:** Имена ключей соответствуют именам свойств с атрибутом `[DataMember]`
- **Enum:** Сохраняется как числовое значение (0 = Open, 1 = Closed, 2 = Bankrupt)
- **Вложенные объекты:** Автоматически сериализуются, если помечены `[DataContract]` и `[DataMember]`

---

## 📝 Этап 7: UI для управления данными

### Задача 6.1: Добавить кнопки в `BankAccountsWindow`
- "Загрузить данные" - загрузка из JSON
- "Сохранить данные" - сохранение в JSON
- "Экспорт в JSON" - сохранение с выбором файла
- "Импорт из JSON" - загрузка с выбором файла

**Рекомендация:** Автоматическое сохранение + кнопка "Сохранить" для ручного сохранения

---

## 📝 Этап 8: Тестирование

### Тестовые сценарии:
1. ✅ Создание счетов → сохранение → закрытие приложения → открытие → проверка загрузки
2. ✅ Пополнение счета → сохранение → проверка баланса после перезапуска
3. ✅ Перевод между счетами → сохранение → проверка балансов
4. ✅ Создание нового клиента → сохранение → проверка загрузки
5. ✅ Обработка отсутствующего файла (создание по умолчанию)
6. ✅ Обработка поврежденного JSON (восстановление или сообщение об ошибке)

---

## 📝 Этап 9: Git - создание ветки

### Задача 8.1: Создать ветку `json`
```bash
git checkout -b json
```

### Задача 8.2: Коммиты
1. "Добавлен класс BankData для контейнера данных"
2. "Добавлен сервис JsonDataService для работы с JSON"
3. "Интегрировано сохранение/загрузка данных в BankAccountsWindow"
4. "Добавлена обработка ошибок при работе с JSON"
5. "Обновлена документация"

### Задача 8.3: Слияние (опционально)
После завершения работы можно смержить ветку в `main`:
```bash
git checkout main
git merge json
```

---

## 🔧 Технические детали

### Используемые библиотеки и классы
- `System.Runtime.Serialization` - библиотека для работы с сериализацией
- `System.Runtime.Serialization.Json.DataContractJsonSerializer` - класс для JSON сериализации
- `System.IO.FileStream` - класс для работы с файловыми потоками

### Основные методы DataContractJsonSerializer
- **Конструктор:** `new DataContractJsonSerializer(typeof(Type))` - создает сериализатор для указанного типа
- **WriteObject(stream, object):** Сериализует объект в поток (запись)
- **ReadObject(stream):** Десериализует данные из потока, возвращает `object` (требуется приведение типа)

### Работа с потоками
- **FileStream с FileMode.Create:** Создает новый файл или перезаписывает существующий
- **FileStream с FileMode.Open:** Открывает существующий файл для чтения
- **Обязательно закрывать поток:** `fileStream.Close()` после использования

### Путь к файлу данных
**Рекомендуется использовать:**
```csharp
string filePath = Environment.CurrentDirectory + @"\bank_data.json";
```
Или:
```csharp
string filePath = Path.Combine(Environment.CurrentDirectory, "bank_data.json");
```

**Альтернативные варианты:**
- `AppData\Local\SimpleBank\bank_data.json` - для продакшн приложений
- `bin\Debug\bank_data.json` - для простоты в учебном проекте

---

## 📊 Последовательность реализации

### День 1: Подготовка
1. ✅ Добавить ссылку на `System.Runtime.Serialization` в проект
2. ✅ Добавить атрибуты `[DataContract]` и `[DataMember]` в классы `Client`, `BankAccount`
3. ✅ Создать класс `BankData` с атрибутами сериализации
4. ✅ Создать папку `Services`

### День 2: Создание сервиса
1. ✅ Создать класс `JsonDataService` с подключением необходимых using
2. ✅ Реализовать метод `SaveToJson` с использованием `DataContractJsonSerializer` и `FileStream`
3. ✅ Реализовать метод `LoadFromJson` с использованием `DataContractJsonSerializer` и `FileStream`
4. ✅ Добавить метод `GetDataFilePath()` для получения пути к файлу
5. ✅ Протестировать на простых данных

### День 3: Интеграция
1. ✅ Модифицировать `BankAccountsWindow` для работы со списком счетов
2. ✅ Добавить загрузку данных при открытии окна
3. ✅ Добавить сохранение при изменениях
4. ✅ Обработать восстановление связей счет-клиент по паспортным данным

### День 4: Обработка связей и ошибок
1. ✅ Реализовать сохранение связи счет-клиент через `OwnerPassportData`
2. ✅ Реализовать восстановление ссылки `Owner` при загрузке
3. ✅ Добавить обработку ошибок (FileNotFoundException, SerializationException)
4. ✅ Протестировать все сценарии

### День 5: Финализация
1. ✅ Добавить UI элементы (кнопки сохранения/загрузки, если требуется)
2. ✅ Создать ветку `json` в Git
3. ✅ Закоммитить все изменения
4. ✅ Написать комментарии в коде

---

## ✅ Чек-лист готовности

- [ ] Добавлена ссылка на `System.Runtime.Serialization` в References
- [ ] Добавлены `using System.Runtime.Serialization.Json;`, `using System.Runtime.Serialization;`, `using System.IO;`
- [ ] Добавлены атрибуты `[DataContract]` в классы `Client`, `BankAccount`, `BankData`
- [ ] Добавлены атрибуты `[DataMember]` для всех сериализуемых свойств
- [ ] Создан класс `BankData` с `List<Client>` и `List<BankAccount>`
- [ ] Создан класс `JsonDataService` с методами `SaveToJson` и `LoadFromJson`
- [ ] Реализовано использование `DataContractJsonSerializer` и `FileStream`
- [ ] Реализована обработка связи счет-клиент через `OwnerPassportData`
- [ ] Модифицирован `BankAccountsWindow` для работы с JSON
- [ ] Реализована загрузка данных при открытии окна
- [ ] Реализовано автоматическое сохранение при изменениях
- [ ] Обработаны ошибки чтения/записи (`try-catch`)
- [ ] Протестированы все сценарии использования
- [ ] Создана ветка `json` в Git
- [ ] Все изменения закоммичены
- [ ] Добавлены комментарии в код

---

## 🎓 Образовательные цели

После выполнения этой работы вы изучите:
1. ✅ Работу с JSON в C# через `DataContractJsonSerializer`
2. ✅ Использование атрибутов `[DataContract]` и `[DataMember]`
3. ✅ Сериализацию и десериализацию объектов в потоки
4. ✅ Работу с `FileStream` и `FileMode` (Create, Open)
5. ✅ Работу с файловой системой через потоки
6. ✅ Обработку ошибок при работе с файлами (`FileNotFoundException`, `SerializationException`)
7. ✅ Работу с Git ветками
8. ✅ Архитектуру сервисов в приложении

---

## 📚 Дополнительные материалы

### Полезные ссылки:
- [DataContractJsonSerializer документация](https://docs.microsoft.com/ru-ru/dotnet/api/system.runtime.serialization.json.datacontractjsonserializer)
- [DataContract атрибут](https://docs.microsoft.com/ru-ru/dotnet/api/system.runtime.serialization.datacontractattribute)
- [FileStream документация](https://docs.microsoft.com/ru-ru/dotnet/api/system.io.filestream)
- [Git ветки](https://git-scm.com/book/ru/v2/Ветвление-в-Git)

### Примеры кода из учебного материала:
- Сериализация одного объекта
- Сериализация массива/списка объектов
- Работа через `FileStream` (файловый поток)
- Работа через `MemoryStream` (поток памяти)

---

## 🚀 Готовы начать?

### Последовательность выполнения:
1. **Этап 2, Задача 2.1** - Добавить ссылку на `System.Runtime.Serialization` в проект
2. **Этап 1, Задача 1.1** - Создать класс `BankData`
3. **Этап 1, Задача 1.2** - Добавить атрибуты `[DataContract]` и `[DataMember]` во все модели
4. **Этап 2, Задача 2.2** - Подключить необходимые using в `JsonDataService`
5. **Этап 3, Задачи 3.1-3.3** - Реализовать методы сохранения и загрузки

---

## 📌 Важные замечания

### Формат даты в JSON
`DataContractJsonSerializer` использует формат `"/Date(миллисекунды+часовой_пояс)/"` вместо стандартного ISO 8601. Это нормально и ожидаемое поведение.

### Порядок атрибутов
Атрибуты `[DataMember]` должны быть перед свойствами, которые нужно сериализовать. Свойства без `[DataMember]` не будут включены в JSON.

### Приведение типов
Метод `ReadObject()` возвращает `object`, поэтому обязательно используйте явное приведение типа:
```csharp
BankData data = (BankData)jsonSerializer.ReadObject(fileStream);
```

### Закрытие потоков
Всегда закрывайте `FileStream` после использования, чтобы освободить ресурсы:
```csharp
fileStream.Close();
```

