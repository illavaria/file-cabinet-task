# File Cabinet

## Описание

Консольное приложение, которое принимает команды пользователя и управляет пользовательскими данными.

### FileCabinetGenerator
Используется для генерации записей. Для работы приложения необходимо наличие в рабочей директории файла конфигураций `validation-rules.json`. Пример файла конфигураций можно найти в репозитории.

Параметры для запуска:

`-t <format>` или `--output-type=<format>`
Обязательный параметр. Определяет формат выходного файла.
Возможные значения:
- `csv` – данные сохраняются в CSV-файле.
- `xml` – данные сохраняются в XML-файле.

`-o <filename>` или `--output=<filename>`
Обязательный параметр. Указывает имя выходного файла, в который будут записаны сгенерированные данные.

`-a <amount>` или `--records-amount=<amount>`
Обязательный параметр. Определяет количество записей, которые нужно сгенерировать.

`-i <start-id>` или `--start-id=<start-id>`
Обязательный параметр. Указывает начальный идентификатор (ID) первой записи.

`-v <validation-rules>` или `--validation=<validation-rules>`
Необязательный параметр. Устанавливает используемые правила валидации.
Возможные значения:

- `default` – стандартные правила валидации.
- `custom` – кастомные правила валидации.

Пример команды для запуска: 

`$ ./FileCabinetGenerator --output-type=xml --output=test.xml --records-amount=1000 --start-id=30`

## FileCabinetApp
## Запуск
Для работы приложения необходимо наличие в рабочей директории файла конфигураций `validation-rules.json`.

Параметры для запуска:

`-v <rule>`  или `--validation-rules=<rule>`
Необязательный параметр. Определяет используемые правила валидации. По умолчанию используются стандартные правила валидации.
Возможные значения:
- `default` – стандартные правила валидации.  
- `custom` – пользовательские правила валидации.  

`-s <type>` или `--storage=<type> ` 
Необязательный параметр. Определяет способ хранения данных. По умолчанию используется хранение в оперативной памяти
- `memory` – хранение данных в оперативной памяти.  
- `file` – хранение данных в файловой системе.  

`-use-stopwatch [-console]` 
Необязательный параметр. Включает измерение времени выполнения команд.  
- Если указан без аргументов, данные записываются в лог-файл.  
- Если дополнительно указан `-console`, время выполнения будет выводиться в консоль.  

`-use-logger` 
Необязательный параметр. Включает запись логов в файл.  

Пример команды для запуска: 

`$ ./FileCabinetApp --validation-rules=custom --storage=file -use-logger -use-stopwatch`

### Команды

- **create** - Creates a new record.  
  **Syntax:** `create`

- **delete** - Deletes a record that satisfies the condition.  
  **Syntax:** `delete where <condition>`

- **edit** - Edits record's data.  
  **Syntax:** `edit <id>`

- **exit** - Exits the application.  
  **Syntax:** `exit`

- **export** - Exports records to a file.  
  **Syntax:** `export <format> <filename>`

- **find** - Prints records with the needed value.  
  **Syntax:** `find <field> <value>`

- **help** - Prints the help screen.  
  **Syntax:** `help [command]`

- **import** - Imports records from a file.  
  **Syntax:** `import <format> <filename>`

- **insert** - Creates a new record with a specified id.  
  **Syntax:** `insert (<fields>) values (<values>)`

- **list** - Prints information about all records.  
  **Syntax:** `list`

- **purge** - Removes deleted records.  
  **Syntax:** `purge`

- **remove** - Removes the record by its id.  
  **Syntax:** `remove <id>`

- **select** - Prints selected fields for records that satisfy the condition.  
  **Syntax:** `select <field1>, <field2> where <field3> = '<value1>' and <field4> = '<value2>'`

- **stat** - Prints the statistics of records.  
  **Syntax:** `stat`

- **update** - Updates some fields of records that satisfy the condition.  
  **Syntax:** `update set <field1> = <value1>, <field2> = <value2> where <field3> = '<value3>' and <field4> = '<value4>'`
