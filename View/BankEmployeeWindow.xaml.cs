using System;
using System.ComponentModel;
using System.Windows;

namespace SimpleBank.View
{
    // Окно функционала сотрудника банка
    public partial class BankEmployeeWindow : Window
    {
        private bool _isBackButtonClicked = false;

        public BankEmployeeWindow()
        {
            InitializeComponent();
        }

        // Метод обработки кнопки "Работа со счетами"
        private void btnBankAccounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BankAccountsWindow bankAccountsWindow = new BankAccountsWindow();
                this.Hide();
                bankAccountsWindow.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна работы со счетами: {ex.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                this.Show();
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
            if (_isBackButtonClicked)
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

