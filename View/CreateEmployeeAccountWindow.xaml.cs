using System;
using System.ComponentModel;
using System.Windows;
using SimpleBank.Services;

namespace SimpleBank.View
{
    // Окно создания нового аккаунта сотрудника
    public partial class CreateEmployeeAccountWindow : Window
    {
        private bool _isCancelButtonClicked = false;

        public CreateEmployeeAccountWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => txtLogin.Focus();
        }

        // Метод обработки кнопки "Создать аккаунт"
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string passwordConfirm = txtPasswordConfirm.Password;
            string fullName = txtFullName.Text.Trim();

            // Валидация полей
            if (string.IsNullOrWhiteSpace(login))
            {
                MessageBox.Show("Пожалуйста, введите логин!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите пароль!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return;
            }

            if (password != passwordConfirm)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPasswordConfirm.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Пожалуйста, введите ФИО сотрудника!", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return;
            }

            // Проверяем, не существует ли уже аккаунт с таким логином
            if (EmployeeAccountService.LoginExists(login))
            {
                MessageBox.Show("Аккаунт с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLogin.Focus();
                return;
            }

            // Создаем новый аккаунт
            bool success = EmployeeAccountService.AddAccount(login, password, fullName);

            if (success)
            {
                MessageBox.Show($"Аккаунт для сотрудника {fullName} успешно создан!\nЛогин: {login}", 
                    "Успех", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                
                _isCancelButtonClicked = true;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка при создании аккаунта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод обработки кнопки "Отмена"
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _isCancelButtonClicked = true;
            this.Close();
        }

        // Метод обработки закрытия окна
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isCancelButtonClicked)
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

