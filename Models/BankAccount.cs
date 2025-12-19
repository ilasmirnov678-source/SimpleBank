using System;

namespace SimpleBank.Models
{
    // Статус банковского счета
    public enum AccountStatus
    {
        Open,
        Closed,
        Bankrupt
    }

    // Класс банковского счета
    public class BankAccount
    {
        private string _accountNumber;
        private DateTime _openingDate;
        private Client _owner;
        private decimal _balance;
        private int _depositTermDays;
        private AccountStatus _status;

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

        public BankAccount()
        {
            _accountNumber = string.Empty;
            _openingDate = DateTime.MinValue;
            _owner = null;
            _balance = 0;
            _depositTermDays = 0;
            _status = AccountStatus.Open;
        }

        public BankAccount(string accountNumber, DateTime openingDate, Client owner, decimal balance, int depositTermDays, AccountStatus status)
        {
            _accountNumber = accountNumber;
            _openingDate = openingDate;
            _owner = owner;
            _balance = balance;
            _depositTermDays = depositTermDays;
            _status = status;
        }

        // Метод расчета даты окончания вклада
        private DateTime CalculateDepositEndDate()
        {
            return _openingDate.AddDays(_depositTermDays);
        }

        // Метод получения даты закрытия счета
        public DateTime GetDepositEndDate()
        {
            return CalculateDepositEndDate();
        }

        // Метод обновления статуса по балансу
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

        // Метод обновления статуса счета
        public void UpdateStatus()
        {
            UpdateStatusByBalance();
        }

        // Метод пополнения счета
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

        // Метод снятия со счета
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

        // Метод перевода средств на другой счет
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

            if (_status != AccountStatus.Open)
            {
                return false;
            }

            if (targetAccount.Status != AccountStatus.Open)
            {
                return false;
            }

            bool withdrawResult = this.Withdraw(amount);
            if (!withdrawResult)
            {
                return false;
            }

            bool depositResult = targetAccount.Deposit(amount);
            if (!depositResult)
            {
                this.Deposit(amount);
                return false;
            }

            return true;
        }

        // Метод вывода информации о счете
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

        // Метод получения строкового представления статуса
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
