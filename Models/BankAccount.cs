using System;

namespace SimpleBank.Models
{
    // Статус банковского счета
    public enum AccountStatus
    {
        Open,      // Открыт
        Closed,    // Закрыт
        Bankrupt   // Банкрот
    }

    // Класс банковского счета
    public class BankAccount
    {
        // Приватные поля
        private string _accountNumber;
        private DateTime _openingDate;
        private Client _owner;
        private decimal _balance;
        private int _depositTermDays;
        private AccountStatus _status;

        // Публичные свойства
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

        // Конструктор по умолчанию
        public BankAccount()
        {
            _accountNumber = string.Empty;
            _openingDate = DateTime.MinValue;
            _owner = null;
            _balance = 0;
            _depositTermDays = 0;
            _status = AccountStatus.Open;
        }

        // Конструктор с параметрами
        public BankAccount(string accountNumber, DateTime openingDate, Client owner, decimal balance, int depositTermDays, AccountStatus status)
        {
            _accountNumber = accountNumber;
            _openingDate = openingDate;
            _owner = owner;
            _balance = balance;
            _depositTermDays = depositTermDays;
            _status = status;
        }

        // Расчет даты окончания вклада
        private DateTime CalculateDepositEndDate()
        {
            return _openingDate.AddDays(_depositTermDays);
        }

        // Получение даты закрытия счета (публичный метод для доступа извне)
        public DateTime GetDepositEndDate()
        {
            return CalculateDepositEndDate();
        }

        // Обновление статуса по балансу
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

        // Обновление статуса счета
        public void UpdateStatus()
        {
            UpdateStatusByBalance();
        }

        // Пополнение счета
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

        // Снятие со счета
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

        // Перевод средств на другой счет
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

            _balance -= amount;
            targetAccount.Balance += amount;
            UpdateStatusByBalance();
            targetAccount.UpdateStatus();

            return true;
        }

        // Вывод информации о счете
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

        // Получение строкового представления статуса
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
