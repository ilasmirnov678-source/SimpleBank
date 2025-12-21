using System;

namespace SimpleBank.Models
{
    // Класс аккаунта сотрудника банка
    public class EmployeeAccount
    {
        private string _login;
        private string _password;
        private string _fullName;

        // Логин сотрудника
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }

        // Пароль сотрудника
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        // Полное имя сотрудника
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        // Конструктор по умолчанию
        public EmployeeAccount()
        {
            _login = string.Empty;
            _password = string.Empty;
            _fullName = string.Empty;
        }

        // Конструктор с параметрами
        public EmployeeAccount(string login, string password, string fullName)
        {
            _login = login;
            _password = password;
            _fullName = fullName;
        }

        // Метод проверки пароля
        public bool CheckPassword(string password)
        {
            return _password == password;
        }
    }
}

