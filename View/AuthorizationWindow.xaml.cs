using System;
using System.ComponentModel;
using System.Windows;

namespace SimpleBank.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private string _userRole;
        private bool _isBackButtonClicked = false;
        private bool _isLoginSuccessful = false;

        /// <summary>
        /// Конструктор окна авторизации
        /// </summary>
        /// <param name="userRole">Роль пользователя: "Employee" или "Administrator"</param>
        public AuthorizationWindow(string userRole)
        {
            InitializeComponent();
            _userRole = userRole;
        }

        /// <summary>
        /// Обработчик кнопки "Войти"
        /// Проверяет логин и пароль, затем открывает соответствующее окно
        /// </summary>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль!",
                                "Ошибка ввода",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Простая проверка авторизации (можно заменить на проверку из базы данных)
            bool isAuthorized = false;

            if (_userRole == "Employee")
            {
                // Для сотрудника: логин = "employee", пароль = "12345"
                isAuthorized = (login.ToLower() == "employee" && password == "12345");
            }
            else if (_userRole == "Administrator")
            {
                // Для администратора: логин = "admin", пароль = "admin123"
                isAuthorized = (login.ToLower() == "admin" && password == "admin123");
            }

            if (isAuthorized)
            {
                // Установить флаг успешной авторизации
                _isLoginSuccessful = true;
                
                // Закрыть окно авторизации (без подтверждения)
                this.DialogResult = true;
                this.Close();

                // Открыть соответствующее окно функционала
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
                                    ? "Для сотрудника: логин = employee, пароль = 12345"
                                    : "Для администратора: логин = admin, пароль = admin123"),
                                "Ошибка авторизации",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                
                // Очистить поле пароля
                txtPassword.Password = "";
                txtLogin.Focus();
            }
        }

        /// <summary>
        /// Обработчик нажатия клавиши в поле логина
        /// При нажатии Enter вызывает обработчик кнопки "Войти"
        /// </summary>
        private void txtLogin_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        /// <summary>
        /// Обработчик нажатия клавиши в поле пароля
        /// При нажатии Enter вызывает обработчик кнопки "Войти"
        /// </summary>
        private void txtPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        /// <summary>
        /// Обработчик кнопки "Назад"
        /// Закрывает окно авторизации и возвращает в главное меню
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _isBackButtonClicked = true;
            this.Close();
        }

        /// <summary>
        /// Обработчик события закрытия окна
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Не показывать подтверждение, если закрытие происходит через кнопку "Назад" или успешную авторизацию
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

