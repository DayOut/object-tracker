# Object tracking
## Завдання 
Потрібно реалізувати frontend+backend застосунок який буде відображати об’єкти на мапі.
- ⁠Сервер віддає об'єкти у вигляді: унікальний ідентифікатор, координати, та напрям руху.
- ⁠Об'єктів може бути одночасно велика кількість (до 100-200)
- Клієнт має відображати їх на мапі
- Якщо сервер перестав віддавати дані по об’єкту, ми його помічаємо як втрачений, а через 5 хв прибираємо з мапи
- Авторизація для перегляду даних про об'єкти має відбуватись по унікальному ключу.⁠
- ⁠Використати WPF, ASP.NET + SignalR

## Запуск додатку
### Запуск сервера:
1. відкрити директорію `ObjectTrackerBackend`
2. виконати 
   ```bash
   docker-compose up --build
   ```

3. залишити консоль відкритою
4. перейти в браузер за адресою [https://localhost:5000/swagger/index.html](https://localhost:5000/swagger/index.html)
5. розгорнути групу ендпоінтів **User**
6. відкрити `/api/user/createTestUser`
7. натиснути **Try it now** 
8. натиснути **Execute**
9. ми отримуємо нашого тестового юзера 
```
   username: admin
   password: admin
   ```

### Запуск клієнта:
1. відкрити директорію `Publish`
2. запустити файл `ObjectTrackingClient.exe`
3. внизу ерану програми ввести логін та пароль

   ```
   username: admin
   password: admin
   ```

4. натиснути `connect`
5. Ви неперевершені!

Подальша логіка роботи додатку:
- при запуску сервера генерується випадковим чином 200 об'єктів
- кожну секунду ці об'єкти зсуваються з однаковою швидкістю, кожен об'єкт в свою сторону
- кожні 10 секунд сервер перестає відправляти 1 об'єкт
- клієнт раз на 3 секунди перевіряє актуальність об'єктів. Якщо об'єкт не присилався сервером протягом 3 секунд з'являється запис `Об'єкт obj-001 було втрачено` в цей момент на карті відображення цього об'єкту стає сірим
- через 5 хвилин після того як об'єкт став сірим - він зникає з карти і робиться запис в лог що об'єкт був видалений

---
# Object tracking
## Task

You need to implement a **frontend + backend** application that displays objects on a map.

- The server provides objects with: a unique identifier, coordinates, and a movement direction.
- There can be a large number of objects simultaneously (up to **100–200**).
- The client must display them on the map.
- If the server stops sending data for an object, it should be marked as **lost**, and after **5 minutes**, removed from the map.
- Authorization to view object data should be done using a **unique key**.
- Use **WPF**, **ASP.NET**, and **SignalR**.

---

## Running the App

### Start the server

1. Open the `ObjectTrackerBackend` directory.
2. Run:

   ```bash
   docker-compose up --build
   ```

3. Keep the console open.
4. Go to: [https://localhost:5000/swagger/index.html](https://localhost:5000/swagger/index.html)
5. Expand the **User** endpoint group.
6. Open `/api/user/createTestUser`.
7. Click **Try it now**.
8. Click **Execute**.
9. You’ll get test user credentials:

   ```
   username: admin
   password: admin
   ```

---

### Start the client

1. Open the `Publish` directory.
2. Launch the `ObjectTrackingClient.exe` file.
3. At the bottom of the app window, enter:

   ```
   username: admin
   password: admin
   ```

4. Click **Connect**.
5. You're amazing!

---

## Application Logic

- On server startup, **200 random objects** are generated.
- Every second, objects move at the same speed, each in its own direction.
- Every 10 seconds, the server **stops sending** one of the objects.
- The client checks object freshness every 3 seconds:
  - If an object hasn't been received from the server within 3 seconds:
    - A log entry appears: `Object obj-001 has been lost`
    - The object turns **gray** on the map
  - After **5 minutes** of being gray:
    - The object disappears from the map
    - A log entry appears: `Object obj-001 has been removed`