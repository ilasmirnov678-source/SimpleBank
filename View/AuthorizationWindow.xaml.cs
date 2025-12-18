using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SimpleBank.View
{
    public partial class AuthorizationWindow : Window
    {
        // Роль пользователя для авторизации: "Employee" (сотрудник) или "Administrator" (администратор)
        private string _userRole;
        // Флаг для отслеживания нажатия кнопки "Назад" - используется для предотвращения подтверждения закрытия
        private bool _isBackButtonClicked = false;
        // Флаг для отслеживания успешной авторизации - используется для предотвращения подтверждения закрытия
        private bool _isLoginSuccessful = false;

        // Конструктор принимает роль пользователя и сохраняет её для проверки авторизации
        public AuthorizationWindow(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;
        }

        // Обработка нажатия кнопки "Войти": проверяет введенные логин и пароль на соответствие учетным данным для роли.
        // Для сотрудника: логин = "emp", пароль = "12345". Для администратора: логин = "admin", пароль = "admin123".
        // При успешной авторизации закрывает окно без подтверждения и открывает соответствующее окно функционала модально.
        // При ошибке показывает сообщение с подсказкой учетных данных, очищает поле пароля и устанавливает фокус на логин
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

        // Обработка нажатия клавиши в поле логина: при нажатии Enter вызывает обработчик кнопки "Войти"
        private void txtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Обработка нажатия клавиши в поле пароля: при нажатии Enter вызывает обработчик кнопки "Войти"
        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        // Обработка нажатия кнопки "Назад": устанавливает флаг и закрывает окно без подтверждения
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _isBackButtonClicked = true;
            this.Close();
        }

        // Обработка события закрытия окна: если закрытие происходит не через кнопку "Назад" или успешную авторизацию,
        // запрашивает подтверждение закрытия. При отказе отменяет закрытие
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

