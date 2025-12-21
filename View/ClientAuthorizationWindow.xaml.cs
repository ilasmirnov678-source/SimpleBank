using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SimpleBank.Models;
using SimpleBank.Services;

namespace SimpleBank.View
{
    // Окно авторизации клиента
    public partial class ClientAuthorizationWindow : Window
    {
        private bool _isBackButtonClicked = false;
        private bool _isLoginSuccessful = false;
        private ClientAccount _currentAccount = null;

        public ClientAuthorizationWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => txtPassportData.Focus();
        }

        // Свойство для получения текущего аккаунта клиента
        public ClientAccount CurrentAccount
        {
            get { return _currentAccount; }
        }

        // Метод обработки кнопки "Войти"
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string passportData = txtPassportData.Text.Trim();
            string pinCode = txtPinCode.Password;

            if (string.IsNullOrEmpty(passportData) || string.IsNullOrEmpty(pinCode))
            {
                MessageBox.Show("Пожалуйста, введите паспортные данные и PIN-код!",
                                "Ошибка ввода",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Проверяем авторизацию через сервис
            ClientAccount account = ClientAccountService.AuthenticateClient(passportData, pinCode);

            if (account != null)
            {
                _isLoginSuccessful = true;
                _currentAccount = account;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверные паспортные данные или PIN-код!\n\n" +
                                "Проверьте правильность введенных данных.",
                                "Ошибка авторизации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                
                txtPinCode.Password = "";
                txtPassportData.Focus();
            }
        }

        // Метод обработки нажатия Enter в поле паспортных данных
        private void txtPassportData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Метод обработки навигации стрелками в поле паспортных данных
        private void txtPassportData_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                txtPinCode.Focus();
                e.Handled = true;
            }
        }

        // Метод обработки нажатия Enter в поле PIN-кода
        private void txtPinCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Метод обработки навигации стрелками в поле PIN-кода
        private void txtPinCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                txtPassportData.Focus();
                txtPassportData.CaretIndex = txtPassportData.Text.Length;
                e.Handled = true;
            }
        }

        // Метод обработки кнопки "Назад"
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _isBackButtonClicked = true;
            this.Close();
        }

        // Метод обработки закрытия окна
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isBackButtonClicked || _isLoginSuccessful)
            {
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите закрыть окно?",
                "Подтверждение закрытия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}

