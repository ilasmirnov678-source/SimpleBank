using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SimpleBank.Models;
using SimpleBank.Services;

namespace SimpleBank.View
{
    // Окно работы с банковскими счетами
    public partial class BankAccountsWindow : Window
    {
        private bool _isBackButtonClicked = false;
        private BankAccount _account1;
        private BankAccount _account2;
        private List<Client> _clients;
        private JsonDataService _dataService;
        private BankData _bankData;

        public BankAccountsWindow()
        {
            InitializeComponent();
            _dataService = new JsonDataService();
            _bankData = new BankData();
            LoadData();
            InitializeControls();
            UpdateAccount1Info();
            UpdateAccount2Info();
            UpdateButtonsState();
        }
        
        // Метод обновления состояния кнопок операций
        private void UpdateButtonsState()
        {
            bool account1Exists = _account1 != null;
            bool account2Exists = _account2 != null;
            
            btnAccount1Deposit.IsEnabled = account1Exists;
            btnAccount1Withdraw.IsEnabled = account1Exists;
            btnAccount1Zero.IsEnabled = account1Exists;
            btnAccount1Bankrupt.IsEnabled = account1Exists;
            btnAccount1Refresh.IsEnabled = account1Exists;
            
            btnAccount2Deposit.IsEnabled = account2Exists;
            btnAccount2Withdraw.IsEnabled = account2Exists;
            btnAccount2Zero.IsEnabled = account2Exists;
            btnAccount2Bankrupt.IsEnabled = account2Exists;
            btnAccount2Refresh.IsEnabled = account2Exists;
        }

        // Метод инициализации элементов управления
        private void InitializeControls()
        {
            dpAccount1OpeningDate.SelectedDate = DateTime.Today;
            dpAccount2OpeningDate.SelectedDate = DateTime.Today;

            cmbAccount1Status.ItemsSource = Enum.GetValues(typeof(AccountStatus));
            cmbAccount1Status.SelectedIndex = 0;
            cmbAccount2Status.ItemsSource = Enum.GetValues(typeof(AccountStatus));
            cmbAccount2Status.SelectedIndex = 0;

            // Если клиенты не загружены из JSON, используем список по умолчанию
            if (_clients == null || _clients.Count == 0)
            {
                _clients = new List<Client>
                {
                    new Client("Кузьмин Олег Иванович", "1234 567890", new DateTime(1988, 3, 15)),
                    new Client("Смирнова Анна Петровна", "2345 678901", new DateTime(1992, 7, 22)),
                    new Client("Волков Дмитрий Сергеевич", "3456 789012", new DateTime(1985, 11, 8)),
                    new Client("Новикова Елена Викторовна", "4567 890123", new DateTime(1990, 1, 30)),
                    new Client("Федоров Максим Александрович", "5678 901234", new DateTime(1987, 9, 14)),
                    new Client("Путин Владимир Владимирович", "0001 000001", new DateTime(1952, 10, 7)),
                    new Client("Эйнштейн Альберт", "9999 999999", new DateTime(1879, 3, 14)),
                    new Client("Гейтс Билл", "8888 888888", new DateTime(1955, 10, 28)),
                    new Client("Маск Илон", "7777 777777", new DateTime(1971, 6, 28)),
                    new Client("Безос Джефф", "6666 666666", new DateTime(1964, 1, 12))
                };
            }

            cmbAccount1Owner.ItemsSource = _clients;
            cmbAccount1Owner.DisplayMemberPath = "FullName";
            cmbAccount2Owner.ItemsSource = _clients;
            cmbAccount2Owner.DisplayMemberPath = "FullName";
            if (_clients.Count > 0)
            {
                cmbAccount1Owner.SelectedIndex = 0;
                cmbAccount2Owner.SelectedIndex = 0;
            }

            cmbTransferFrom.SelectedIndex = 0;
            cmbTransferTo.SelectedIndex = 1;
        }

        // Метод загрузки данных из JSON
        private void LoadData()
        {
            try
            {
                string filePath = _dataService.GetDataFilePath();
                _bankData = _dataService.LoadFromJson(filePath);

                // Загружаем клиентов
                if (_bankData.Clients != null && _bankData.Clients.Count > 0)
                {
                    _clients = _bankData.Clients;
                }

                // Загружаем счета и восстанавливаем связи с клиентами
                if (_bankData.Accounts != null && _bankData.Accounts.Count > 0)
                {
                    foreach (var account in _bankData.Accounts)
                    {
                        // Восстанавливаем связь с клиентом по паспортным данным
                        if (!string.IsNullOrEmpty(account.OwnerPassportData) && _clients != null)
                        {
                            account.Owner = _clients.FirstOrDefault(c => c.PassportData == account.OwnerPassportData);
                        }
                    }

                    // Присваиваем первые два счета для UI
                    if (_bankData.Accounts.Count > 0)
                    {
                        _account1 = _bankData.Accounts[0];
                    }
                    if (_bankData.Accounts.Count > 1)
                    {
                        _account2 = _bankData.Accounts[1];
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", 
                    "Ошибка загрузки", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Warning);
                
                // Инициализируем пустые данные при ошибке
                _bankData = new BankData();
                _clients = new List<Client>();
            }
        }

        // Метод сохранения данных в JSON
        private void SaveData()
        {
            try
            {
                // Собираем все счета в список
                List<BankAccount> accounts = new List<BankAccount>();
                if (_account1 != null)
                {
                    // Устанавливаем OwnerPassportData для сериализации
                    _account1.OwnerPassportData = _account1.Owner?.PassportData ?? string.Empty;
                    accounts.Add(_account1);
                }
                if (_account2 != null)
                {
                    // Устанавливаем OwnerPassportData для сериализации
                    _account2.OwnerPassportData = _account2.Owner?.PassportData ?? string.Empty;
                    accounts.Add(_account2);
                }

                // Обновляем данные для сохранения
                _bankData.Clients = _clients ?? new List<Client>();
                _bankData.Accounts = accounts;

                // Сохраняем в JSON
                string filePath = _dataService.GetDataFilePath();
                _dataService.SaveToJson(_bankData, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", 
                    "Ошибка сохранения", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
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
                SaveData();
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
            else
            {
                SaveData();
            }
        }

        // Метод обработки создания первого счета
        private void btnAccount1Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetAccountFieldsHighlight(1);
                bool isValid = true;
                if (string.IsNullOrWhiteSpace(txtAccount1Number.Text) || txtAccount1Number.Text.Trim().Length != 20)
                {
                    HighlightInvalidField(txtAccount1Number);
                    isValid = false;
                }

                if (!dpAccount1OpeningDate.SelectedDate.HasValue)
                {
                    HighlightInvalidField(dpAccount1OpeningDate);
                    isValid = false;
                }

                if (cmbAccount1Owner.SelectedItem == null)
                {
                    HighlightInvalidField(cmbAccount1Owner);
                    isValid = false;
                }

                string balanceText = txtAccount1Balance.Text.Replace(",", ".");
                if (!decimal.TryParse(balanceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
                {
                    HighlightInvalidField(txtAccount1Balance);
                    isValid = false;
                }

                if (!int.TryParse(txtAccount1DepositTerm.Text, out int depositTerm) || depositTerm < 0)
                {
                    HighlightInvalidField(txtAccount1DepositTerm);
                    isValid = false;
                }

                if (cmbAccount1Status.SelectedItem == null)
                {
                    HighlightInvalidField(cmbAccount1Status);
                    isValid = false;
                }

                if (!isValid)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Client owner = (Client)cmbAccount1Owner.SelectedItem;
                AccountStatus status = (AccountStatus)cmbAccount1Status.SelectedItem;
                DateTime openingDate = dpAccount1OpeningDate.SelectedDate.Value;

                _account1 = new BankAccount(
                    txtAccount1Number.Text.Trim(),
                    openingDate,
                    owner,
                    balance,
                    depositTerm,
                    status
                );

                UpdateAccount1Info();
                ResetAccountFieldsHighlight(1);
                UpdateButtonsState();
                SaveData();

                MessageBox.Show("Счет успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании счета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод обработки пополнения первого счета
        private void btnAccount1Deposit_Click(object sender, RoutedEventArgs e)
        {
            ResetFieldHighlight(txtAccount1Amount);
            
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string amountText = txtAccount1Amount.Text.Replace(",", ".");
            if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                HighlightInvalidField(txtAccount1Amount);
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account1.Deposit(amount);
            if (result)
            {
                UpdateAccount1Info();
                txtAccount1Amount.Text = "";
                SaveData();
                MessageBox.Show($"Счет успешно пополнен на {FormatMoney(amount)}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при пополнении счета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод обработки снятия средств с первого счета
        private void btnAccount1Withdraw_Click(object sender, RoutedEventArgs e)
        {
            ResetFieldHighlight(txtAccount1Amount);
            
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string amountText = txtAccount1Amount.Text.Replace(",", ".");
            if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                HighlightInvalidField(txtAccount1Amount);
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account1.Withdraw(amount);
            if (result)
            {
                UpdateAccount1Info();
                txtAccount1Amount.Text = "";
                SaveData();
                MessageBox.Show($"Со счета успешно снято {FormatMoney(amount)}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

        // Метод обработки обнуления первого счета
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
            SaveData();
            MessageBox.Show("Баланс счета обнулен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Метод проверки банкротства первого счета
        private void btnAccount1Bankrupt_Click(object sender, RoutedEventArgs e)
        {
            if (_account1 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isBankrupt = _account1.Status == AccountStatus.Bankrupt;
            string message = isBankrupt 
                ? $"Счет является банкротом.\nБаланс: {FormatMoney(_account1.Balance)}"
                : $"Счет не является банкротом.\nСтатус: {_account1.Status}\nБаланс: {FormatMoney(_account1.Balance)}";

            MessageBox.Show(message, "Проверка банкротства", MessageBoxButton.OK, 
                isBankrupt ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        // Метод обработки обновления информации о первом счете
        private void btnAccount1Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateAccount1Info();
        }

        // Метод расчета даты закрытия счета
        private DateTime CalculateClosingDate(BankAccount account)
        {
            return account.GetDepositEndDate();
        }

        // Метод расчета дней до закрытия счета
        private int CalculateDaysUntilClosing(BankAccount account)
        {
            DateTime closingDate = CalculateClosingDate(account);
            int daysLeft = (int)(closingDate - DateTime.Today).TotalDays;
            return daysLeft >= 0 ? daysLeft : 0;
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
            lblAccount1Balance.Text = FormatMoney(_account1.Balance);
            lblAccount1Status.Text = GetStatusString(_account1.Status);
            lblAccount1OpeningDate.Text = _account1.OpeningDate.ToString("dd.MM.yyyy");

            DateTime closingDate = CalculateClosingDate(_account1);
            lblAccount1ClosingDate.Text = closingDate.ToString("dd.MM.yyyy");

            int daysLeft = CalculateDaysUntilClosing(_account1);
            lblAccount1DaysLeft.Text = daysLeft.ToString();
        }

        // Метод обработки создания второго счета
        private void btnAccount2Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ResetAccountFieldsHighlight(2);
                bool isValid = true;
                if (string.IsNullOrWhiteSpace(txtAccount2Number.Text) || txtAccount2Number.Text.Trim().Length != 20)
                {
                    HighlightInvalidField(txtAccount2Number);
                    isValid = false;
                }

                if (!dpAccount2OpeningDate.SelectedDate.HasValue)
                {
                    HighlightInvalidField(dpAccount2OpeningDate);
                    isValid = false;
                }

                if (cmbAccount2Owner.SelectedItem == null)
                {
                    HighlightInvalidField(cmbAccount2Owner);
                    isValid = false;
                }

                string balanceText = txtAccount2Balance.Text.Replace(",", ".");
                if (!decimal.TryParse(balanceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balance))
                {
                    HighlightInvalidField(txtAccount2Balance);
                    isValid = false;
                }

                if (!int.TryParse(txtAccount2DepositTerm.Text, out int depositTerm) || depositTerm < 0)
                {
                    HighlightInvalidField(txtAccount2DepositTerm);
                    isValid = false;
                }

                if (cmbAccount2Status.SelectedItem == null)
                {
                    HighlightInvalidField(cmbAccount2Status);
                    isValid = false;
                }

                if (!isValid)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля корректно!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                Client owner = (Client)cmbAccount2Owner.SelectedItem;
                AccountStatus status = (AccountStatus)cmbAccount2Status.SelectedItem;
                DateTime openingDate = dpAccount2OpeningDate.SelectedDate.Value;

                _account2 = new BankAccount(
                    txtAccount2Number.Text.Trim(),
                    openingDate,
                    owner,
                    balance,
                    depositTerm,
                    status
                );

                UpdateAccount2Info();
                ResetAccountFieldsHighlight(2);
                UpdateButtonsState();
                SaveData();

                MessageBox.Show("Счет успешно создан!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании счета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод обработки пополнения второго счета
        private void btnAccount2Deposit_Click(object sender, RoutedEventArgs e)
        {
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string amountText = txtAccount2Amount.Text.Replace(",", ".");
            if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                HighlightInvalidField(txtAccount2Amount);
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account2.Deposit(amount);
            if (result)
            {
                UpdateAccount2Info();
                txtAccount2Amount.Text = "";
                SaveData();
                MessageBox.Show($"Счет успешно пополнен на {FormatMoney(amount)}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ошибка при пополнении счета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод обработки снятия средств со второго счета
        private void btnAccount2Withdraw_Click(object sender, RoutedEventArgs e)
        {
            ResetFieldHighlight(txtAccount2Amount);
            
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string amountText = txtAccount2Amount.Text.Replace(",", ".");
            if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                HighlightInvalidField(txtAccount2Amount);
                MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool result = _account2.Withdraw(amount);
            if (result)
            {
                UpdateAccount2Info();
                txtAccount2Amount.Text = "";
                SaveData();
                MessageBox.Show($"Со счета успешно снято {FormatMoney(amount)}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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

        // Метод обработки обнуления второго счета
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
            SaveData();
            MessageBox.Show("Баланс счета обнулен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Метод проверки банкротства второго счета
        private void btnAccount2Bankrupt_Click(object sender, RoutedEventArgs e)
        {
            if (_account2 == null)
            {
                MessageBox.Show("Сначала создайте счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool isBankrupt = _account2.Status == AccountStatus.Bankrupt;
            string message = isBankrupt 
                ? $"Счет является банкротом.\nБаланс: {FormatMoney(_account2.Balance)}"
                : $"Счет не является банкротом.\nСтатус: {_account2.Status}\nБаланс: {FormatMoney(_account2.Balance)}";

            MessageBox.Show(message, "Проверка банкротства", MessageBoxButton.OK, 
                isBankrupt ? MessageBoxImage.Warning : MessageBoxImage.Information);
        }

        // Метод обработки обновления информации о втором счете
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
            lblAccount2Balance.Text = FormatMoney(_account2.Balance);
            lblAccount2Status.Text = GetStatusString(_account2.Status);
            lblAccount2OpeningDate.Text = _account2.OpeningDate.ToString("dd.MM.yyyy");

            DateTime closingDate = CalculateClosingDate(_account2);
            lblAccount2ClosingDate.Text = closingDate.ToString("dd.MM.yyyy");

            int daysLeft = CalculateDaysUntilClosing(_account2);
            lblAccount2DaysLeft.Text = daysLeft.ToString();
        }

        // Метод получения строкового представления статуса
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

        // Метод обработки перевода средств между счетами
        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_account1 == null || _account2 == null)
                {
                    MessageBox.Show("Сначала создайте оба счета!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lblTransferResult.Text = "Ошибка: не созданы оба счета";
                    return;
                }

                if (cmbTransferFrom.SelectedItem == null || cmbTransferTo.SelectedItem == null)
                {
                    MessageBox.Show("Выберите счет отправителя и получателя!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lblTransferResult.Text = "Ошибка: не выбраны счета";
                    return;
                }

                string fromTag = ((System.Windows.Controls.ComboBoxItem)cmbTransferFrom.SelectedItem).Tag.ToString();
                string toTag = ((System.Windows.Controls.ComboBoxItem)cmbTransferTo.SelectedItem).Tag.ToString();

                if (fromTag == toTag)
                {
                    MessageBox.Show("Нельзя переводить средства на тот же счет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lblTransferResult.Text = "Ошибка: выбран один и тот же счет";
                    return;
                }

                BankAccount fromAccount = (fromTag == "Account1") ? _account1 : _account2;
                BankAccount toAccount = (toTag == "Account1") ? _account1 : _account2;

                ResetFieldHighlight(txtTransferAmount);
                string transferAmountText = txtTransferAmount.Text.Replace(",", ".");
                if (!decimal.TryParse(transferAmountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    HighlightInvalidField(txtTransferAmount);
                    MessageBox.Show("Введите корректную положительную сумму!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    lblTransferResult.Text = "Ошибка: неверная сумма";
                    return;
                }

                bool result = fromAccount.Transfer(toAccount, amount);

                if (result)
                {
                    UpdateAccount1Info();
                    UpdateAccount2Info();
                    txtTransferAmount.Text = "";
                    SaveData();
                    
                    string fromAccountName = (fromTag == "Account1") ? "Счет 1" : "Счет 2";
                    string toAccountName = (toTag == "Account1") ? "Счет 1" : "Счет 2";
                    lblTransferResult.Text = $"Успешно переведено {FormatMoney(amount)} с {fromAccountName} на {toAccountName}";
                    lblTransferResult.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 128, 0));
                    
                    MessageBox.Show($"Перевод выполнен успешно!\n\nС {fromAccountName} на {toAccountName}\nСумма: {FormatMoney(amount)}", 
                                    "Успех", 
                                    MessageBoxButton.OK, 
                                    MessageBoxImage.Information);
                }
                else
                {
                    string errorMessage = "Ошибка при переводе средств";
                    if (fromAccount.Status != AccountStatus.Open)
                    {
                        errorMessage = "Счет отправителя не открыт!";
                    }
                    else if (toAccount.Status != AccountStatus.Open)
                    {
                        errorMessage = "Счет получателя не открыт!";
                    }
                    else if (fromAccount.Balance < amount)
                    {
                        errorMessage = "Недостаточно средств на счете отправителя!";
                    }
                    else
                    {
                        errorMessage = "Неизвестная ошибка при переводе!";
                    }

                    lblTransferResult.Text = errorMessage;
                    lblTransferResult.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 20, 60));
                    
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении перевода: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                lblTransferResult.Text = $"Ошибка: {ex.Message}";
                lblTransferResult.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 20, 60));
            }
        }

        // Метод подсветки невалидного поля
        private void HighlightInvalidField(Control control)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
                textBox.BorderThickness = new Thickness(2);
                textBox.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xE5, 0xE5));
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
                comboBox.BorderThickness = new Thickness(2);
                comboBox.Background = new SolidColorBrush(Color.FromRgb(0xFF, 0xE5, 0xE5));
            }
            else if (control is DatePicker datePicker)
            {
                datePicker.BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0x6B, 0x6B));
                datePicker.BorderThickness = new Thickness(2);
            }
        }

        // Метод сброса подсветки поля
        private void ResetFieldHighlight(Control control)
        {
            if (control is TextBox textBox)
            {
                textBox.BorderBrush = (SolidColorBrush)Application.Current.Resources["MatteBlueBrush"];
                textBox.BorderThickness = new Thickness(2);
                textBox.Background = (SolidColorBrush)Application.Current.Resources["WhiteBrush"];
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BorderBrush = (SolidColorBrush)Application.Current.Resources["MatteBlueBrush"];
                comboBox.BorderThickness = new Thickness(2);
                comboBox.Background = (SolidColorBrush)Application.Current.Resources["WhiteBrush"];
            }
            else if (control is DatePicker datePicker)
            {
                datePicker.BorderBrush = (SolidColorBrush)Application.Current.Resources["MatteBlueBrush"];
                datePicker.BorderThickness = new Thickness(2);
            }
        }

        // Метод сброса подсветки всех полей счета
        private void ResetAccountFieldsHighlight(int accountNumber)
        {
            if (accountNumber == 1)
            {
                ResetFieldHighlight(txtAccount1Number);
                ResetFieldHighlight(dpAccount1OpeningDate);
                ResetFieldHighlight(cmbAccount1Owner);
                ResetFieldHighlight(txtAccount1Balance);
                ResetFieldHighlight(txtAccount1DepositTerm);
                ResetFieldHighlight(cmbAccount1Status);
            }
            else
            {
                ResetFieldHighlight(txtAccount2Number);
                ResetFieldHighlight(dpAccount2OpeningDate);
                ResetFieldHighlight(cmbAccount2Owner);
                ResetFieldHighlight(txtAccount2Balance);
                ResetFieldHighlight(txtAccount2DepositTerm);
                ResetFieldHighlight(cmbAccount2Status);
            }
        }

        // Метод форматирования денежной суммы
        private string FormatMoney(decimal amount)
        {
            return $"{amount:F2} ₽";
        }

        // Метод валидации номера счета
        private bool ValidateAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
            {
                return false;
            }
            string trimmed = accountNumber.Trim();
            return trimmed.Length == 20 && trimmed.All(char.IsDigit);
        }

        // Метод обработки изменения текста в поле номера счета 1
        private void txtAccount1Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (ValidateAccountNumber(textBox.Text))
                {
                    ResetFieldHighlight(textBox);
                }
                else if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    HighlightInvalidField(textBox);
                }
            }
        }

        // Метод обработки ввода текста в поле номера счета 1
        private void txtAccount1Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        // Метод обработки нажатия клавиш в поле номера счета 1
        private void txtAccount1Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    string digitsOnly = new string(clipboardText.Where(char.IsDigit).ToArray());
                    if (digitsOnly.Length > 20)
                    {
                        digitsOnly = digitsOnly.Substring(0, 20);
                    }
                    ((TextBox)sender).Text = digitsOnly;
                    ((TextBox)sender).CaretIndex = digitsOnly.Length;
                    e.Handled = true;
                }
            }
        }

        // Метод обработки потери фокуса в поле номера счета 1
        private void txtAccount1Number_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (!ValidateAccountNumber(textBox.Text) && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    HighlightInvalidField(textBox);
                }
                else if (ValidateAccountNumber(textBox.Text))
                {
                    ResetFieldHighlight(textBox);
                }
            }
        }

        // Метод обработки изменения текста в поле номера счета 2
        private void txtAccount2Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (ValidateAccountNumber(textBox.Text))
                {
                    ResetFieldHighlight(textBox);
                }
                else if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    HighlightInvalidField(textBox);
                }
            }
        }

        // Метод обработки ввода текста в поле номера счета 2
        private void txtAccount2Number_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        // Метод обработки нажатия клавиш в поле номера счета 2
        private void txtAccount2Number_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    string digitsOnly = new string(clipboardText.Where(char.IsDigit).ToArray());
                    if (digitsOnly.Length > 20)
                    {
                        digitsOnly = digitsOnly.Substring(0, 20);
                    }
                    ((TextBox)sender).Text = digitsOnly;
                    ((TextBox)sender).CaretIndex = digitsOnly.Length;
                    e.Handled = true;
                }
            }
        }

        // Метод обработки потери фокуса в поле номера счета 2
        private void txtAccount2Number_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (!ValidateAccountNumber(textBox.Text) && !string.IsNullOrWhiteSpace(textBox.Text))
                {
                    HighlightInvalidField(textBox);
                }
                else if (ValidateAccountNumber(textBox.Text))
                {
                    ResetFieldHighlight(textBox);
                }
            }
        }

        // Метод генерации случайного номера счета
        private string GenerateRandomAccountNumber()
        {
            Random random = new Random();
            const string digits = "0123456789";
            char[] accountNumber = new char[20];
            
            for (int i = 0; i < 20; i++)
            {
                accountNumber[i] = digits[random.Next(digits.Length)];
            }
            
            return new string(accountNumber);
        }

        // Метод обработки генерации номера счета 1
        private void btnGenerateAccount1Number_Click(object sender, RoutedEventArgs e)
        {
            string randomNumber = GenerateRandomAccountNumber();
            txtAccount1Number.Text = randomNumber;
            ResetFieldHighlight(txtAccount1Number);
        }

        // Метод обработки генерации номера счета 2
        private void btnGenerateAccount2Number_Click(object sender, RoutedEventArgs e)
        {
            string randomNumber = GenerateRandomAccountNumber();
            txtAccount2Number.Text = randomNumber;
            ResetFieldHighlight(txtAccount2Number);
        }

        // Метод валидации ввода для денежных полей
        private void ValidateDecimalInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            string currentText = textBox.Text;
            
            if (char.IsDigit(e.Text, 0))
            {
                e.Handled = false;
                return;
            }

            if (e.Text == "." || e.Text == ",")
            {
                if (!currentText.Contains(".") && !currentText.Contains(","))
                {
                    if (e.Text == ",")
                    {
                        e.Handled = true;
                        textBox.Text = currentText.Insert(textBox.CaretIndex, ".");
                        textBox.CaretIndex = textBox.CaretIndex + 1;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = true;
                }
                return;
            }

            e.Handled = true;
        }

        // Метод обработки вставки для денежных полей
        private void HandleDecimalPaste(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null && Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    clipboardText = clipboardText.Replace(",", ".");
                    string cleanedText = new string(clipboardText.Where(c => char.IsDigit(c) || c == '.').ToArray());
                    int firstDotIndex = cleanedText.IndexOf('.');
                    if (firstDotIndex >= 0)
                    {
                        cleanedText = cleanedText.Substring(0, firstDotIndex + 1) + 
                                     cleanedText.Substring(firstDotIndex + 1).Replace(".", "");
                    }
                    
                    textBox.Text = cleanedText;
                    textBox.CaretIndex = cleanedText.Length;
                    e.Handled = true;
                }
            }
        }

        // Метод обработки ввода текста в поле баланса счета 1
        private void txtAccount1Balance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateDecimalInput(sender, e);
        }

        // Метод обработки нажатия клавиш в поле баланса счета 1
        private void txtAccount1Balance_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleDecimalPaste(sender, e);
        }

        // Метод обработки ввода текста в поле суммы операции счета 1
        private void txtAccount1Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateDecimalInput(sender, e);
        }

        // Метод обработки нажатия клавиш в поле суммы операции счета 1
        private void txtAccount1Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleDecimalPaste(sender, e);
        }

        // Метод обработки ввода текста в поле баланса счета 2
        private void txtAccount2Balance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateDecimalInput(sender, e);
        }

        // Метод обработки нажатия клавиш в поле баланса счета 2
        private void txtAccount2Balance_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleDecimalPaste(sender, e);
        }

        // Метод обработки ввода текста в поле суммы операции счета 2
        private void txtAccount2Amount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateDecimalInput(sender, e);
        }

        // Метод обработки нажатия клавиш в поле суммы операции счета 2
        private void txtAccount2Amount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleDecimalPaste(sender, e);
        }

        // Метод обработки ввода текста в поле суммы перевода
        private void txtTransferAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ValidateDecimalInput(sender, e);
        }

        // Метод обработки нажатия клавиш в поле суммы перевода
        private void txtTransferAmount_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            HandleDecimalPaste(sender, e);
        }
    }
}

