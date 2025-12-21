using System;
using System.Runtime.Serialization;

namespace SimpleBank.Models
{
    // Тип операции
    public enum TransactionType
    {
        Deposit,    // Пополнение
        Withdraw,   // Снятие
        Transfer    // Перевод
    }

    // Класс транзакции (операции)
    [DataContract]
    public class Transaction
    {
        private string _transactionId;
        private string _accountNumber;
        private TransactionType _transactionType;
        private decimal _amount;
        private DateTime _transactionDate;
        private string _description;
        private string _targetAccountNumber;

        [DataMember]
        public string TransactionId
        {
            get { return _transactionId; }
            set { _transactionId = value; }
        }

        [DataMember]
        public string AccountNumber
        {
            get { return _accountNumber; }
            set { _accountNumber = value; }
        }

        [DataMember]
        public TransactionType TransactionType
        {
            get { return _transactionType; }
            set { _transactionType = value; }
        }

        [DataMember]
        public decimal Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        [DataMember]
        public DateTime TransactionDate
        {
            get { return _transactionDate; }
            set { _transactionDate = value; }
        }

        [DataMember]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        [DataMember]
        public string TargetAccountNumber
        {
            get { return _targetAccountNumber; }
            set { _targetAccountNumber = value; }
        }

        // Конструктор по умолчанию
        public Transaction()
        {
            _transactionId = Guid.NewGuid().ToString();
            _accountNumber = string.Empty;
            _transactionType = TransactionType.Deposit;
            _amount = 0;
            _transactionDate = DateTime.Now;
            _description = string.Empty;
            _targetAccountNumber = string.Empty;
        }

        // Конструктор с параметрами
        public Transaction(string accountNumber, TransactionType transactionType, decimal amount, string description, string targetAccountNumber = "")
        {
            _transactionId = Guid.NewGuid().ToString();
            _accountNumber = accountNumber;
            _transactionType = transactionType;
            _amount = amount;
            _transactionDate = DateTime.Now;
            _description = description ?? string.Empty;
            _targetAccountNumber = targetAccountNumber ?? string.Empty;
        }

        // Метод получения строкового представления типа операции
        public string GetTransactionTypeString()
        {
            switch (_transactionType)
            {
                case TransactionType.Deposit:
                    return "Пополнение";
                case TransactionType.Withdraw:
                    return "Снятие";
                case TransactionType.Transfer:
                    return "Перевод";
                default:
                    return "Неизвестно";
            }
        }
    }
}

