using System;

namespace SimpleBank.Models
{
    // Перечисление для статуса банковского счета
    public enum AccountStatus
    {
        Open,      // Открыт
        Closed,    // Закрыт
        Bankrupt   // Банкрот
    }

    // Класс для представления банковского счета
    public class BankAccount
    {
        // Приватные поля класса
        private string _accountNumber;
        private DateTime _openingDate;
        private Client _owner;
        private decimal _balance;
        private int _depositTermDays;
        private AccountStatus _status;

        // Публичные свойства для доступа к полям
        public string AccountNumber
        {
            get { return _accountNumber; }
            set { _accountNumber = value; }
        }

        public DateTime OpeningDate
        {
            get { return _openingDate; }
            set { _openingDate = value; }
        }

        public Client Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        public decimal Balance
        {
            get { return _balance; }
            set { _balance = value; }
        }

        public int DepositTermDays
        {
            get { return _depositTermDays; }
            set { _depositTermDays = value; }
        }

        public AccountStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        // Конструктор по умолчанию: инициализирует все поля значениями по умолчанию
        public BankAccount()
        {
            _accountNumber = string.Empty;
            _openingDate = DateTime.MinValue;
            _owner = null;
            _balance = 0;
            _depositTermDays = 0;
            _status = AccountStatus.Open;
        }

        // Конструктор с параметрами: принимает все необходимые параметры для инициализации счета
        public BankAccount(string accountNumber, DateTime openingDate, Client owner, decimal balance, int depositTermDays, AccountStatus status)
        {
            _accountNumber = accountNumber;
            _openingDate = openingDate;
            _owner = owner;
            _balance = balance;
            _depositTermDays = depositTermDays;
            _status = status;
        }

        // Приватный метод: определение даты окончания вклада
        // Возвращает дату открытия плюс срок вклада в днях
        private DateTime CalculateDepositEndDate()
        {
            return _openingDate.AddDays(_depositTermDays);
        }

        // Приватный метод: изменение статуса вклада в зависимости от суммы на счете
        // Если баланс < 0 → статус "банкрот", если баланс = 0 и счет закрыт → "закрыт", иначе "открыт"
        private void UpdateStatusByBalance()
        {
            if (_balance < 0)
            {
                _status = AccountStatus.Bankrupt;
            }
            else if (_balance == 0 && _status == AccountStatus.Closed)
            {
                _status = AccountStatus.Closed;
            }
            else if (_balance >= 0 && _status != AccountStatus.Closed)
            {
                _status = AccountStatus.Open;
            }
        }

        // Публичный метод для обновления статуса счета (используется при переводе средств на другой счет)
        public void UpdateStatus()
        {
            UpdateStatusByBalance();
        }

        // Публичный метод: пополнение счета
        // Проверяет, что сумма пополнения > 0, увеличивает баланс и обновляет статус
        // Возвращает true при успехе, false при ошибке
        public bool Deposit(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            _balance += amount;
            UpdateStatusByBalance();
            return true;
        }

        // Публичный метод: снятие со счета с проверкой возможности снятия
        // Проверяет: amount > 0, Balance >= amount, Status == Open
        // Уменьшает баланс и обновляет статус
        // Возвращает true при успехе, false при ошибке
        public bool Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (_balance < amount)
            {
                return false;
            }

            if (_status != AccountStatus.Open)
            {
                return false;
            }

            _balance -= amount;
            UpdateStatusByBalance();
            return true;
        }

        // Публичный метод: перевод средств на другой счет с проверкой
        // Проверяет: amount > 0, targetAccount != null, targetAccount != this,
        // Balance >= amount, Status == Open, targetAccount.Status == Open
        // Выполняет снятие с текущего счета и пополнение целевого счета
        // Возвращает true при успехе, false при ошибке
        public bool Transfer(BankAccount targetAccount, decimal amount)
        {
            if (amount <= 0)
            {
                return false;
            }

            if (targetAccount == null)
            {
                return false;
            }

            if (targetAccount == this)
            {
                return false;
            }

            if (_balance < amount)
            {
                return false;
            }

            if (_status != AccountStatus.Open)
            {
                return false;
            }

            if (targetAccount.Status != AccountStatus.Open)
            {
                return false;
            }

            // Выполняем перевод: снимаем с текущего счета и пополняем целевой
            _balance -= amount;
            targetAccount.Balance += amount;

            // Обновляем статусы обоих счетов
            UpdateStatusByBalance();
            targetAccount.UpdateStatus();

            return true;
        }

        // Метод вывода полной информации о счете, включая данные владельца
        public void DisplayInfo()
        {
            Console.WriteLine("=== Информация о банковском счете ===");
            Console.WriteLine($"Номер счета: {_accountNumber}");
            Console.WriteLine($"Дата открытия: {_openingDate:dd.MM.yyyy}");
            Console.WriteLine($"Срок вклада (дни): {_depositTermDays}");
            
            DateTime endDate = CalculateDepositEndDate();
            Console.WriteLine($"Дата окончания вклада: {endDate:dd.MM.yyyy}");
            
            Console.WriteLine($"Баланс: {_balance:F2}");
            Console.WriteLine($"Статус: {GetStatusString(_status)}");
            
            if (_owner != null)
            {
                Console.WriteLine("\n--- Данные владельца ---");
                _owner.DisplayInfo();
            }
            else
            {
                Console.WriteLine("\nВладелец не указан");
            }
            
            Console.WriteLine("=====================================");
        }

        // Вспомогательный метод для получения строкового представления статуса
        private string GetStatusString(AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Open:
                    return "Открыт";
                case AccountStatus.Closed:
                    return "Закрыт";
                case AccountStatus.Bankrupt:
                    return "Банкрот";
                default:
                    return "Неизвестно";
            }
        }
    }
}
