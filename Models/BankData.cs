using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SimpleBank.Models
{
    // Контейнер для всех данных банка (клиенты и счета)
    [DataContract]
    public class BankData
    {
        private List<Client> _clients;
        private List<BankAccount> _accounts;

        [DataMember]
        public List<Client> Clients
        {
            get { return _clients; }
            set { _clients = value; }
        }

        [DataMember]
        public List<BankAccount> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; }
        }

        // Конструктор по умолчанию
        public BankData()
        {
            _clients = new List<Client>();
            _accounts = new List<BankAccount>();
        }

        // Конструктор с параметрами
        public BankData(List<Client> clients, List<BankAccount> accounts)
        {
            _clients = clients ?? new List<Client>();
            _accounts = accounts ?? new List<BankAccount>();
        }
    }
}

