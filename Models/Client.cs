using System;
using System.Runtime.Serialization;

namespace SimpleBank.Models
{
    // Класс клиента банка
    [DataContract]
    public class Client
    {
        private string _fullName;
        private string _passportData;
        private DateTime _dateOfBirth;

        [DataMember]
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        [DataMember]
        public string PassportData
        {
            get { return _passportData; }
            set { _passportData = value; }
        }

        [DataMember]
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value; }
        }

        public Client()
        {
            _fullName = string.Empty;
            _passportData = string.Empty;
            _dateOfBirth = DateTime.MinValue;
        }

        public Client(string fullName, string passportData, DateTime dateOfBirth)
        {
            _fullName = fullName;
            _passportData = passportData;
            _dateOfBirth = dateOfBirth;
        }

        // Метод вывода информации о клиенте
        public void DisplayInfo()
        {
            Console.WriteLine("=== Информация о клиенте ===");
            Console.WriteLine($"ФИО: {_fullName}");
            Console.WriteLine($"Данные паспорта: {_passportData}");
            Console.WriteLine($"Дата рождения: {_dateOfBirth:dd.MM.yyyy}");
            Console.WriteLine("============================");
        }
    }
}

