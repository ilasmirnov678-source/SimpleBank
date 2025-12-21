using System.Collections.Generic;
using System.Linq;
using SimpleBank.Models;

namespace SimpleBank.Services
{
    // Сервис для управления аккаунтами сотрудников банка
    public class EmployeeAccountService
    {
        private static List<EmployeeAccount> _employeeAccounts;

        // Инициализация списка аккаунтов сотрудников
        static EmployeeAccountService()
        {
            _employeeAccounts = new List<EmployeeAccount>
            {
                new EmployeeAccount("emp", "123", "Иванов Иван Иванович"),
                new EmployeeAccount("petrov", "456", "Петров Петр Петрович"),
                new EmployeeAccount("sidorov", "789", "Сидоров Сидор Сидорович")
            };
        }

        // Метод получения всех аккаунтов
        public static List<EmployeeAccount> GetAllAccounts()
        {
            return _employeeAccounts;
        }

        // Метод проверки авторизации
        public static EmployeeAccount Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Ищем аккаунт по логину (без учета регистра)
            EmployeeAccount account = _employeeAccounts.FirstOrDefault(
                a => a.Login.ToLower() == login.ToLower());

            if (account == null)
            {
                return null;
            }

            // Проверяем пароль
            if (account.CheckPassword(password))
            {
                return account;
            }

            return null;
        }

        // Метод добавления нового аккаунта
        public static bool AddAccount(string login, string password, string fullName)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
            {
                return false;
            }

            // Проверяем, не существует ли уже аккаунт с таким логином
            if (_employeeAccounts.Any(a => a.Login.ToLower() == login.ToLower()))
            {
                return false;
            }

            // Добавляем новый аккаунт
            _employeeAccounts.Add(new EmployeeAccount(login, password, fullName));
            return true;
        }

        // Метод проверки существования логина
        public static bool LoginExists(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return false;
            }

            return _employeeAccounts.Any(a => a.Login.ToLower() == login.ToLower());
        }
    }
}

