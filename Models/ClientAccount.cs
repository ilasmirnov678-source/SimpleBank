using System.Runtime.Serialization;

namespace SimpleBank.Models
{
    // Модель учетной записи клиента
    [DataContract]
    public class ClientAccount
    {
        private string _passportData;
        private string _pinCode;
        private string _fullName;

        [DataMember]
        public string PassportData
        {
            get { return _passportData; }
            set { _passportData = value; }
        }

        [DataMember]
        public string PinCode
        {
            get { return _pinCode; }
            set { _pinCode = value; }
        }

        [DataMember]
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        // Конструктор по умолчанию
        public ClientAccount()
        {
            _passportData = string.Empty;
            _pinCode = string.Empty;
            _fullName = string.Empty;
        }

        // Конструктор с параметрами
        public ClientAccount(string passportData, string pinCode, string fullName)
        {
            _passportData = passportData;
            _pinCode = pinCode;
            _fullName = fullName;
        }

        // Метод проверки PIN-кода
        public bool CheckPinCode(string pinCode)
        {
            return _pinCode == pinCode;
        }
    }
}

