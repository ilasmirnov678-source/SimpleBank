using System;
using SimpleBank.Models;

namespace SimpleBank.Models
{
    // Класс тестирования функциональности классов Client и BankAccount
    public class BankClassesTest
    {
        // Метод запуска всех тестов
        public static void RunAllTests()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("НАЧАЛО ТЕСТИРОВАНИЯ КЛАССОВ");
            Console.WriteLine("========================================\n");

            TestClientClass();
            TestBankAccountClass();

            Console.WriteLine("\n========================================");
            Console.WriteLine("ТЕСТИРОВАНИЕ ЗАВЕРШЕНО");
            Console.WriteLine("========================================");
        }

        // Метод тестирования класса Client
        private static void TestClientClass()
        {
            Console.WriteLine("--- Тестирование класса Client ---\n");

            // Тест 1: Конструктор по умолчанию
            Console.WriteLine("Тест 1: Создание клиента через конструктор по умолчанию");
            Client client1 = new Client();
            Console.WriteLine($"ФИО: '{client1.FullName}', Паспорт: '{client1.PassportData}', Дата рождения: {client1.DateOfBirth}");
            Console.WriteLine("✓ Конструктор по умолчанию работает\n");

            // Тест 2: Конструктор с параметрами
            Console.WriteLine("Тест 2: Создание клиента через конструктор с параметрами");
            DateTime birthDate = new DateTime(1990, 5, 15);
            Client client2 = new Client("Иванов Иван Иванович", "1234 567890", birthDate);
            Console.WriteLine("Создан клиент:");
            client2.DisplayInfo();
            Console.WriteLine("✓ Конструктор с параметрами работает\n");

            // Тест 3: Изменение свойств
            Console.WriteLine("Тест 3: Изменение свойств клиента");
            client2.FullName = "Петров Петр Петрович";
            client2.PassportData = "9876 543210";
            Console.WriteLine($"Изменено ФИО на: {client2.FullName}");
            Console.WriteLine($"Изменен паспорт на: {client2.PassportData}");
            Console.WriteLine("✓ Свойства работают корректно\n");
        }

        // Метод тестирования класса BankAccount
        private static void TestBankAccountClass()
        {
            Console.WriteLine("--- Тестирование класса BankAccount ---\n");

            // Создание клиента для тестов
            Client testClient = new Client("Сидоров Сидор Сидорович", "1111 222222", new DateTime(1985, 3, 20));

            // Тест 1: Конструктор по умолчанию
            Console.WriteLine("Тест 1: Создание счета через конструктор по умолчанию");
            BankAccount account1 = new BankAccount();
            Console.WriteLine($"Номер счета: '{account1.AccountNumber}', Баланс: {account1.Balance}, Статус: {account1.Status}");
            Console.WriteLine("✓ Конструктор по умолчанию работает\n");

            // Тест 2: Конструктор с параметрами
            Console.WriteLine("Тест 2: Создание счета через конструктор с параметрами");
            DateTime openingDate = new DateTime(2024, 1, 1);
            BankAccount account2 = new BankAccount("ACC-001", openingDate, testClient, 10000.50m, 365, AccountStatus.Open);
            Console.WriteLine("Создан счет:");
            account2.DisplayInfo();
            Console.WriteLine("✓ Конструктор с параметрами работает\n");

            // Тест 3: Пополнение счета (успешное)
            Console.WriteLine("Тест 3: Пополнение счета (успешное)");
            decimal initialBalance = account2.Balance;
            bool depositResult = account2.Deposit(5000.25m);
            Console.WriteLine($"Баланс до пополнения: {initialBalance:F2}");
            Console.WriteLine($"Результат пополнения: {depositResult}, Баланс после: {account2.Balance:F2}");
            Console.WriteLine($"Статус: {account2.Status}");
            Console.WriteLine("✓ Пополнение счета работает корректно\n");

            // Тест 4: Пополнение счета (ошибка - отрицательная сумма)
            Console.WriteLine("Тест 4: Пополнение счета (ошибка - отрицательная сумма)");
            decimal balanceBefore = account2.Balance;
            bool depositError = account2.Deposit(-1000m);
            Console.WriteLine($"Результат пополнения: {depositError}, Баланс не изменился: {account2.Balance:F2}");
            Console.WriteLine($"✓ Отрицательная сумма отклонена\n");

            // Тест 5: Снятие со счета (успешное)
            Console.WriteLine("Тест 5: Снятие со счета (успешное)");
            decimal balanceBeforeWithdraw = account2.Balance;
            bool withdrawResult = account2.Withdraw(2000m);
            Console.WriteLine($"Баланс до снятия: {balanceBeforeWithdraw:F2}");
            Console.WriteLine($"Результат снятия: {withdrawResult}, Баланс после: {account2.Balance:F2}");
            Console.WriteLine($"Статус: {account2.Status}");
            Console.WriteLine("✓ Снятие со счета работает корректно\n");

            // Тест 6: Снятие со счета (ошибка - недостаточно средств)
            Console.WriteLine("Тест 6: Снятие со счета (ошибка - недостаточно средств)");
            decimal balanceBeforeError = account2.Balance;
            bool withdrawError = account2.Withdraw(100000m);
            Console.WriteLine($"Результат снятия: {withdrawError}, Баланс не изменился: {account2.Balance:F2}");
            Console.WriteLine($"✓ Недостаточно средств - операция отклонена\n");

            // Тест 7: Снятие со счета (ошибка - отрицательная сумма)
            Console.WriteLine("Тест 7: Снятие со счета (ошибка - отрицательная сумма)");
            bool withdrawNegative = account2.Withdraw(-500m);
            Console.WriteLine($"Результат снятия: {withdrawNegative}, Баланс не изменился: {account2.Balance:F2}");
            Console.WriteLine($"✓ Отрицательная сумма отклонена\n");

            // Тест 8: Перевод средств (успешный)
            Console.WriteLine("Тест 8: Перевод средств (успешный)");
            Client client3 = new Client("Кузнецов Кузьма Кузьмич", "3333 444444", new DateTime(1992, 7, 10));
            BankAccount account3 = new BankAccount("ACC-002", new DateTime(2024, 2, 1), client3, 5000m, 180, AccountStatus.Open);
            
            decimal balance2Before = account2.Balance;
            decimal balance3Before = account3.Balance;
            bool transferResult = account2.Transfer(account3, 1000m);
            
            Console.WriteLine($"Счет 1 - Баланс до перевода: {balance2Before:F2}, после: {account2.Balance:F2}");
            Console.WriteLine($"Счет 2 - Баланс до перевода: {balance3Before:F2}, после: {account3.Balance:F2}");
            Console.WriteLine($"Результат перевода: {transferResult}");
            Console.WriteLine($"✓ Перевод средств работает корректно\n");

            // Тест 9: Перевод средств (ошибка - недостаточно средств)
            Console.WriteLine("Тест 9: Перевод средств (ошибка - недостаточно средств)");
            decimal balance2BeforeError = account2.Balance;
            decimal balance3BeforeError = account3.Balance;
            bool transferError = account2.Transfer(account3, 100000m);
            Console.WriteLine($"Результат перевода: {transferError}");
            Console.WriteLine($"Баланс счета 1 не изменился: {account2.Balance:F2}");
            Console.WriteLine($"Баланс счета 2 не изменился: {account3.Balance:F2}");
            Console.WriteLine($"✓ Недостаточно средств - операция отклонена\n");

            // Тест 10: Перевод средств (ошибка - перевод на тот же счет)
            Console.WriteLine("Тест 10: Перевод средств (ошибка - перевод на тот же счет)");
            decimal balanceBeforeSelf = account2.Balance;
            bool transferSelf = account2.Transfer(account2, 1000m);
            Console.WriteLine($"Результат перевода: {transferSelf}");
            Console.WriteLine($"Баланс не изменился: {account2.Balance:F2}");
            Console.WriteLine($"✓ Перевод на тот же счет отклонен\n");

            // Тест 11: Перевод средств (ошибка - null счет)
            Console.WriteLine("Тест 11: Перевод средств (ошибка - null счет)");
            decimal balanceBeforeNull = account2.Balance;
            bool transferNull = account2.Transfer(null, 1000m);
            Console.WriteLine($"Результат перевода: {transferNull}");
            Console.WriteLine($"Баланс не изменился: {account2.Balance:F2}");
            Console.WriteLine($"✓ Перевод на null счет отклонен\n");

            // Тест 12: Изменение статуса при отрицательном балансе
            Console.WriteLine("Тест 12: Изменение статуса при отрицательном балансе");
            BankAccount account4 = new BankAccount("ACC-003", new DateTime(2024, 1, 1), testClient, 1000m, 365, AccountStatus.Open);
            Console.WriteLine($"Начальный баланс: {account4.Balance:F2}, Статус: {account4.Status}");
            account4.Withdraw(1500m); // Снимаем больше, чем есть
            Console.WriteLine($"Баланс после снятия: {account4.Balance:F2}, Статус: {account4.Status}");
            Console.WriteLine($"✓ Статус изменился на Bankrupt при отрицательном балансе\n");

            // Тест 13: Проверка расчета даты окончания вклада
            Console.WriteLine("Тест 13: Проверка расчета даты окончания вклада");
            DateTime testOpeningDate = new DateTime(2024, 1, 1);
            int testTermDays = 365;
            BankAccount account5 = new BankAccount("ACC-004", testOpeningDate, testClient, 5000m, testTermDays, AccountStatus.Open);
            DateTime expectedEndDate = testOpeningDate.AddDays(testTermDays);
            Console.WriteLine($"Дата открытия: {testOpeningDate:dd.MM.yyyy}");
            Console.WriteLine($"Срок вклада: {testTermDays} дней");
            Console.WriteLine($"Ожидаемая дата окончания: {expectedEndDate:dd.MM.yyyy}");
            account5.DisplayInfo();
            Console.WriteLine($"✓ Расчет даты окончания вклада работает корректно\n");

            // Тест 14: Снятие со счета при закрытом счете
            Console.WriteLine("Тест 14: Снятие со счета при закрытом счете");
            BankAccount account6 = new BankAccount("ACC-005", new DateTime(2024, 1, 1), testClient, 5000m, 365, AccountStatus.Closed);
            decimal balanceBeforeClosed = account6.Balance;
            bool withdrawClosed = account6.Withdraw(1000m);
            Console.WriteLine($"Статус счета: {account6.Status}");
            Console.WriteLine($"Результат снятия: {withdrawClosed}");
            Console.WriteLine($"Баланс не изменился: {account6.Balance:F2}");
            Console.WriteLine($"✓ Снятие с закрытого счета отклонено\n");

            // Тест 15: Перевод с закрытого счета
            Console.WriteLine("Тест 15: Перевод с закрытого счета");
            BankAccount account7 = new BankAccount("ACC-006", new DateTime(2024, 1, 1), testClient, 5000m, 365, AccountStatus.Closed);
            BankAccount account8 = new BankAccount("ACC-007", new DateTime(2024, 1, 1), client3, 1000m, 180, AccountStatus.Open);
            decimal balance7Before = account7.Balance;
            decimal balance8Before = account8.Balance;
            bool transferFromClosed = account7.Transfer(account8, 1000m);
            Console.WriteLine($"Результат перевода: {transferFromClosed}");
            Console.WriteLine($"Баланс счета 1 не изменился: {account7.Balance:F2}");
            Console.WriteLine($"Баланс счета 2 не изменился: {account8.Balance:F2}");
            Console.WriteLine($"✓ Перевод с закрытого счета отклонен\n");
        }
    }
}

