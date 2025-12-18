using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SimpleBank.Models;

namespace SimpleBank.View
{
    public partial class BankAccountsWindow : Window
    {
        // Флаг нажатия кнопки "Назад"
        private bool _isBackButtonClicked = false;
        // Объекты счетов
        private BankAccount _account1;
        private BankAccount _account2;
        // Список клиентов для ComboBox
        private List<Client> _clients;

        public BankAccountsWindow()
        {
            InitializeComponent();
            InitializeControls();
        }

        // Инициализация элементов управления
        private void InitializeControls()
        {
            // Установка текущей даты по умолчанию
            dpAccount1OpeningDate.SelectedDate = DateTime.Today;
            dpAccount2OpeningDate.SelectedDate = DateTime.Today;

            // Заполнение ComboBox статусов
            cmbAccount1Status.ItemsSource = Enum.GetValues(typeof(AccountStatus));
            cmbAccount1Status.SelectedIndex = 0; // Open по умолчанию
            cmbAccount2Status.ItemsSource = Enum.GetValues(typeof(AccountStatus));
            cmbAccount2Status.SelectedIndex = 0; // Open по умолчанию

            // Создание тестовых клиентов для выбора
            _clients = new List<Client>
            {
                new Client("Иванов Иван Иванович", "1234 567890", new DateTime(1990, 5, 15)),
                new Client("Петров Петр Петрович", "9876 543210", new DateTime(1985, 3, 20)),
                new Client("Сидоров Сидор Сидорович", "1111 222222", new DateTime(1992, 7, 10))
            };

            // Заполнение ComboBox владельцев
            cmbAccount1Owner.ItemsSource = _clients;
            cmbAccount1Owner.DisplayMemberPath = "FullName";
            cmbAccount2Owner.ItemsSource = _clients;
            cmbAccount2Owner.DisplayMemberPath = "FullName";
            if (_clients.Count > 0)
            {
                cmbAccount1Owner.SelectedIndex = 0;
                cmbAccount2Owner.SelectedIndex = 0;
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

        // Заглушки для обработчиков (будут реализованы на следующих этапах)
        private void btnAccount1Create_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount1Deposit_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount1Withdraw_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount1Zero_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount1Bankrupt_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount1Refresh_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        // Заглушки для обработчиков второго счета (будут реализованы на следующих этапах)
        private void btnAccount2Create_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount2Deposit_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount2Withdraw_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount2Zero_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount2Bankrupt_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }

        private void btnAccount2Refresh_Click(object sender, RoutedEventArgs e)
        {
            // Будет реализовано на Этапе 5
        }
    }
}

