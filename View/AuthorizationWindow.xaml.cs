using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SimpleBank.View
{
    // Окно авторизации пользователя
    public partial class AuthorizationWindow : Window
    {
        private string _userRole;
        private bool _isBackButtonClicked = false;
        private bool _isLoginSuccessful = false;

        public AuthorizationWindow(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;
            this.Loaded += (s, e) => txtLogin.Focus();
        }

        // Метод обработки кнопки "Войти"
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
                isAuthorized = (login.ToLower() == "emp" && password == "123");
            }
            else if (_userRole == "Administrator")
            {
                isAuthorized = (login.ToLower() == "adm" && password == "123");
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
                                        ? "Для сотрудника: логин = emp, пароль = 123"
                                        : "Для администратора: логин = adm, пароль = 123"),
                                    "Ошибка авторизации",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                
                txtPassword.Password = "";
                txtLogin.Focus();
            }
        }

        // Метод обработки нажатия Enter в поле логина
        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Метод обработки навигации стрелками в поле логина
        private void txtLogin_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                txtPassword.Focus();
                e.Handled = true;
            }
        }

        // Метод обработки нажатия Enter в поле пароля
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Метод обработки навигации стрелками в поле пароля
        private void txtPassword_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                txtLogin.Focus();
                txtLogin.CaretIndex = txtLogin.Text.Length;
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

