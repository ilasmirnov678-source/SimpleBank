using System.Collections.Generic;
using System.Linq;
using SimpleBank.Models;

namespace SimpleBank.Services
{
    // Сервис для управления учетными записями клиентов
    public class ClientAccountService
    {
        private static List<ClientAccount> _clientAccounts;

        // Инициализация списка аккаунтов клиентов
        static ClientAccountService()
        {
            _clientAccounts = new List<ClientAccount>
            {
                new ClientAccount("1234 567890", "1111", "Кузьмин Олег Иванович"),
                new ClientAccount("2345 678901", "2222", "Смирнова Анна Петровна"),
                new ClientAccount("3456 789012", "3333", "Волков Дмитрий Сергеевич"),
                new ClientAccount("4567 890123", "4444", "Новикова Елена Викторовна"),
                new ClientAccount("5678 901234", "5555", "Федоров Максим Александрович")
            };
        }

        // Метод получения всех аккаунтов
        public static List<ClientAccount> GetAllAccounts()
        {
            return _clientAccounts;
        }

        // Метод аутентификации клиента
        public static ClientAccount AuthenticateClient(string passportData, string pinCode)
        {
            if (string.IsNullOrWhiteSpace(passportData) || string.IsNullOrWhiteSpace(pinCode))
            {
                return null;
            }

            // Ищем аккаунт по паспортным данным
            ClientAccount account = _clientAccounts.FirstOrDefault(
                a => a.PassportData.Trim() == passportData.Trim());

            if (account == null)
            {
                return null;
            }

            // Проверяем PIN-код
            if (account.CheckPinCode(pinCode))
            {
                return account;
            }

            return null;
        }

        // Метод создания нового аккаунта клиента
        public static bool CreateClientAccount(string passportData, string pinCode, string fullName)
        {
            if (string.IsNullOrWhiteSpace(passportData) || string.IsNullOrWhiteSpace(pinCode) || string.IsNullOrWhiteSpace(fullName))
            {
                return false; // Все поля должны быть заполнены
            }

            // Проверяем, не существует ли уже аккаунт с такими паспортными данными
            if (_clientAccounts.Any(a => a.PassportData.Trim() == passportData.Trim()))
            {
                return false; // Аккаунт с такими паспортными данными уже существует
            }

            // Добавляем новый аккаунт
            _clientAccounts.Add(new ClientAccount(passportData, pinCode, fullName));
            return true;
        }

        // Метод проверки существования паспортных данных
        public static bool IsPassportExists(string passportData)
        {
            if (string.IsNullOrWhiteSpace(passportData))
            {
                return false;
            }

            return _clientAccounts.Any(a => a.PassportData.Trim() == passportData.Trim());
        }
    }
}

