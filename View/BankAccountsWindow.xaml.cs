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

        // Создание первого счета
        private void btnAccount1Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация полей
                if (string.IsNullOrWhiteSpace(txtAccount1Number.Text))
                {
                    MessageBox.Show("Введите номер счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!dpAccount1OpeningDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату открытия счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbAccount1Owner.SelectedItem == null)
                {
                    MessageBox.Show("Выберите владельца счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAccount1Balance.Text, out decimal balance))
                {
                    MessageBox.Show("Введите корректную сумму баланса!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtAccount1DepositTerm.Text, out int depositTerm) || depositTerm < 0)
                {
                    MessageBox.Show("Введите корректный срок вклада (положительное число)!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbAccount1Status.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получение данных
                Client owner = (Client)cmbAccount1Owner.SelectedItem;
                AccountStatus status = (AccountStatus)cmbAccount1Status.SelectedItem;
                DateTime openingDate = dpAccount1OpeningDate.SelectedDate.Value;

                // Создание объекта счета
                _account1 = new BankAccount(
                    txtAccount1Number.Text.Trim(),
                    openingDate,
                    owner,
                    balance,
                    depositTerm,
                    status
                );

                // Обновление информации
                UpdateAccount1Info();

                MessageBox.Show("Счет успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании счета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Пополнение первого счета
        private void btnAccount1Deposit_Click(object sender, RoutedEventArgs e)
        {
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAccount1Amount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account1.Deposit(amount);
            if (result)
            {
                UpdateAccount1Info();
                txtAccount1Amount.Text = "";
                MessageBox.Show($"Счет успешно пополнен на {amount:F2}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при пополнении счета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Снятие с первого счета
        private void btnAccount1Withdraw_Click(object sender, RoutedEventArgs e)
        {
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAccount1Amount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account1.Withdraw(amount);
            if (result)
            {
                UpdateAccount1Info();
                txtAccount1Amount.Text = "";
                MessageBox.Show($"Со счета успешно снято {amount:F2}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (_account1.Balance < amount)
                {
                    MessageBox.Show("Недостаточно средств на счете!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (_account1.Status != AccountStatus.Open)
                {
                    MessageBox.Show("Счет не открыт! Невозможно снять средства.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Ошибка при снятии средств!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обнуление первого счета
        private void btnAccount1Zero_Click(object sender, RoutedEventArgs e)
        {
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _account1.Balance = 0;
            _account1.UpdateStatus();
            UpdateAccount1Info();
            MessageBox.Show("Баланс счета обнулен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Проверка банкротства первого счета
        private void btnAccount1Bankrupt_Click(object sender, RoutedEventArgs e)
        {
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isBankrupt = _account1.Status == AccountStatus.Bankrupt;
            string message = isBankrupt 
                ? $"Счет является банкротом.\nБаланс: {_account1.Balance:F2}"
                : $"Счет не является банкротом.\nСтатус: {_account1.Status}\nБаланс: {_account1.Balance:F2}";

            MessageBox.Show(message, "Проверка банкротства", MessageBoxButton.OK, 
                isBankrupt ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        // Обновление информации о первом счете
        private void btnAccount1Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccount1Info();
        }

        // Метод обновления информации о первом счете
        private void UpdateAccount1Info()
        {
            if (_account1 == null)
            {
                lblAccount1Number.Text = "—";
                lblAccount1Balance.Text = "—";
                lblAccount1Status.Text = "—";
                lblAccount1OpeningDate.Text = "—";
                lblAccount1ClosingDate.Text = "—";
                lblAccount1DaysLeft.Text = "—";
                return;
            }

            lblAccount1Number.Text = _account1.AccountNumber;
            lblAccount1Balance.Text = $"{_account1.Balance:F2}";
            lblAccount1Status.Text = GetStatusString(_account1.Status);
            lblAccount1OpeningDate.Text = _account1.OpeningDate.ToString("dd.MM.yyyy");

            DateTime closingDate = _account1.GetDepositEndDate();
            lblAccount1ClosingDate.Text = closingDate.ToString("dd.MM.yyyy");

            int daysLeft = (int)(closingDate - DateTime.Today).TotalDays;
            lblAccount1DaysLeft.Text = daysLeft >= 0 ? daysLeft.ToString() : "0";
        }

        // Создание второго счета
        private void btnAccount2Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация полей
                if (string.IsNullOrWhiteSpace(txtAccount2Number.Text))
                {
                    MessageBox.Show("Введите номер счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!dpAccount2OpeningDate.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату открытия счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbAccount2Owner.SelectedItem == null)
                {
                    MessageBox.Show("Выберите владельца счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtAccount2Balance.Text, out decimal balance))
                {
                    MessageBox.Show("Введите корректную сумму баланса!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtAccount2DepositTerm.Text, out int depositTerm) || depositTerm < 0)
                {
                    MessageBox.Show("Введите корректный срок вклада (положительное число)!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbAccount2Status.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус счета!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Получение данных
                Client owner = (Client)cmbAccount2Owner.SelectedItem;
                AccountStatus status = (AccountStatus)cmbAccount2Status.SelectedItem;
                DateTime openingDate = dpAccount2OpeningDate.SelectedDate.Value;

                // Создание объекта счета
                _account2 = new BankAccount(
                    txtAccount2Number.Text.Trim(),
                    openingDate,
                    owner,
                    balance,
                    depositTerm,
                    status
                );

                // Обновление информации
                UpdateAccount2Info();

                MessageBox.Show("Счет успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании счета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Пополнение второго счета
        private void btnAccount2Deposit_Click(object sender, RoutedEventArgs e)
        {
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAccount2Amount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account2.Deposit(amount);
            if (result)
            {
                UpdateAccount2Info();
                txtAccount2Amount.Text = "";
                MessageBox.Show($"Счет успешно пополнен на {amount:F2}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при пополнении счета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Снятие со второго счета
        private void btnAccount2Withdraw_Click(object sender, RoutedEventArgs e)
        {
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtAccount2Amount.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account2.Withdraw(amount);
            if (result)
            {
                UpdateAccount2Info();
                txtAccount2Amount.Text = "";
                MessageBox.Show($"Со счета успешно снято {amount:F2}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (_account2.Balance < amount)
                {
                    MessageBox.Show("Недостаточно средств на счете!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (_account2.Status != AccountStatus.Open)
                {
                    MessageBox.Show("Счет не открыт! Невозможно снять средства.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Ошибка при снятии средств!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Обнуление второго счета
        private void btnAccount2Zero_Click(object sender, RoutedEventArgs e)
        {
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _account2.Balance = 0;
            _account2.UpdateStatus();
            UpdateAccount2Info();
            MessageBox.Show("Баланс счета обнулен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Проверка банкротства второго счета
        private void btnAccount2Bankrupt_Click(object sender, RoutedEventArgs e)
        {
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isBankrupt = _account2.Status == AccountStatus.Bankrupt;
            string message = isBankrupt 
                ? $"Счет является банкротом.\nБаланс: {_account2.Balance:F2}"
                : $"Счет не является банкротом.\nСтатус: {_account2.Status}\nБаланс: {_account2.Balance:F2}";

            MessageBox.Show(message, "Проверка банкротства", MessageBoxButton.OK, 
                isBankrupt ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        // Обновление информации о втором счете
        private void btnAccount2Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccount2Info();
        }

        // Метод обновления информации о втором счете
        private void UpdateAccount2Info()
        {
            if (_account2 == null)
            {
                lblAccount2Number.Text = "—";
                lblAccount2Balance.Text = "—";
                lblAccount2Status.Text = "—";
                lblAccount2OpeningDate.Text = "—";
                lblAccount2ClosingDate.Text = "—";
                lblAccount2DaysLeft.Text = "—";
                return;
            }

            lblAccount2Number.Text = _account2.AccountNumber;
            lblAccount2Balance.Text = $"{_account2.Balance:F2}";
            lblAccount2Status.Text = GetStatusString(_account2.Status);
            lblAccount2OpeningDate.Text = _account2.OpeningDate.ToString("dd.MM.yyyy");

            DateTime closingDate = _account2.GetDepositEndDate();
            lblAccount2ClosingDate.Text = closingDate.ToString("dd.MM.yyyy");

            int daysLeft = (int)(closingDate - DateTime.Today).TotalDays;
            lblAccount2DaysLeft.Text = daysLeft >= 0 ? daysLeft.ToString() : "0";
        }

        // Вспомогательный метод для получения строкового представления статуса
        private string GetStatusString(AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Open:
                    return "Открыт";
                case AccountStatus.Closed:
                    return "Закрыт";
                case AccountStatus.Bankrupt:
                    return "Банкрот";
                default:
                    return "Неизвестно";
            }
        }
    }
}

