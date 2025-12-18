using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SimpleBank.View
{
    public partial class AuthorizationWindow : Window
    {
        // Роль пользователя
        private string _userRole;
        // Флаг нажатия кнопки "Назад"
        private bool _isBackButtonClicked = false;
        // Флаг успешной авторизации
        private bool _isLoginSuccessful = false;

        public AuthorizationWindow(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;
        }

        // Обработчик кнопки "Войти"
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль!",
                                "Ошибка ввода",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            bool isAuthorized = false;

            if (_userRole == "Employee")
            {
                isAuthorized = (login.ToLower() == "emp" && password == "12345");
            }
            else if (_userRole == "Administrator")
            {
                isAuthorized = (login.ToLower() == "admin" && password == "admin123");
            }

            if (isAuthorized)
            {
                _isLoginSuccessful = true;
                this.DialogResult = true;
                this.Close();

                try
                {
                    if (_userRole == "Employee")
                    {
                        BankEmployeeWindow employeeWindow = new BankEmployeeWindow();
                        employeeWindow.ShowDialog();
                    }
                    else if (_userRole == "Administrator")
                    {
                        SystemAdministratorWindow adminWindow = new SystemAdministratorWindow();
                        adminWindow.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии окна: {ex.Message}",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!\n\n" +
                                (_userRole == "Employee" 
                                    ? "Для сотрудника: логин = emp, пароль = 12345"
                                    : "Для администратора: логин = admin, пароль = admin123"),
                                "Ошибка авторизации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                
                txtPassword.Password = "";
                txtLogin.Focus();
            }
        }

        // Обработчик нажатия Enter в поле логина
        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Обработчик нажатия Enter в поле пароля
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Обработчик кнопки "Назад"
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _isBackButtonClicked = true;
            this.Close();
        }

        // Обработчик закрытия окна
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

