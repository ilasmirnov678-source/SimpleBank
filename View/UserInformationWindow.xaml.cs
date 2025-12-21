using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using SimpleBank.Models;
using SimpleBank.Services;

namespace SimpleBank.View
{
    // Окно информации для клиента
    public partial class UserInformationWindow : Window
    {
        private bool _isBackButtonClicked = false;
        private JsonDataService _dataService;
        private BankData _bankData;
        private List<Client> _clients;
        private List<BankAccount> _accounts;
        private ClientAccount _currentClientAccount;
        private List<BankAccount> _clientAccounts; // Счета текущего клиента
        private List<BankAccount> _filteredAccounts; // Отфильтрованные счета для отображения
        private List<Transaction> _allTransactions; // Все транзакции по счетам клиента
        private List<Transaction> _filteredTransactions; // Отфильтрованные транзакции для отображения
        private bool _validationWarningShown = false; // Флаг для показа предупреждения о валидации

        // Конструктор с авторизованным клиентом
        public UserInformationWindow(ClientAccount clientAccount)
        {
            InitializeComponent();
            _currentClientAccount = clientAccount;
            _dataService = new JsonDataService();
            _bankData = new BankData();
            _clientAccounts = new List<BankAccount>();
            _filteredAccounts = new List<BankAccount>();
            _allTransactions = new List<Transaction>();
            _filteredTransactions = new List<Transaction>();
            LoadData();
            FilterClientAccounts();
            FilterAccounts();
            LoadTransactions();
            FilterTransactions();
            UpdateUI();
        }

        // Конструктор без авторизации (для обратной совместимости)
        public UserInformationWindow()
        {
            InitializeComponent();
            _currentClientAccount = null;
            _dataService = new JsonDataService();
            _bankData = new BankData();
            _clientAccounts = new List<BankAccount>();
            _filteredAccounts = new List<BankAccount>();
            _allTransactions = new List<Transaction>();
            _filteredTransactions = new List<Transaction>();
            LoadData();
            FilterAccounts();
            LoadTransactions();
            FilterTransactions();
            UpdateUI();
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

        // Метод загрузки данных из JSON
        private void LoadData()
        {
            try
            {
                string filePath = _dataService.GetDataFilePath();
                
                // Проверяем существование файла перед загрузкой
                if (!System.IO.File.Exists(filePath))
                {
                    // Файл не существует - это нормально для первого запуска
                    InitializeEmptyData();
                    return;
                }

                // Загружаем данные
                _bankData = _dataService.LoadFromJson(filePath);

                // Валидация загруженных данных
                if (!ValidateBankData(_bankData))
                {
                    // Данные некорректны, используем пустые данные
                    InitializeEmptyData();
                    MessageBox.Show(
                        "Обнаружены некорректные данные в файле. Приложение будет работать с пустыми данными.\n\n" +
                        "Рекомендуется проверить файл bank_data.json или создать резервную копию.",
                        "Предупреждение о данных",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Загружаем клиентов
                if (_bankData.Clients != null && _bankData.Clients.Count > 0)
                {
                    _clients = _bankData.Clients;
                }
                else
                {
                    _clients = new List<Client>();
                }

                // Загружаем счета и восстанавливаем связи с клиентами
                if (_bankData.Accounts != null && _bankData.Accounts.Count > 0)
                {
                    _accounts = _bankData.Accounts;

                    // Восстанавливаем связи между счетами и клиентами
                    foreach (var account in _accounts)
                    {
                        // Восстанавливаем связь с клиентом по паспортным данным
                        if (!string.IsNullOrEmpty(account.OwnerPassportData) && _clients != null)
                        {
                            account.Owner = _clients.FirstOrDefault(c => c.PassportData == account.OwnerPassportData);
                        }
                    }
                }
                else
                {
                    _accounts = new List<BankAccount>();
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // Файл не найден - это нормально для первого запуска
                InitializeEmptyData();
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                // Директория не найдена
                InitializeEmptyData();
                ShowError("Ошибка доступа к файлу",
                    $"Директория для сохранения данных не найдена.\n\n" +
                    $"Путь: {ex.Message}\n\n" +
                    $"Приложение будет работать без сохранения данных.",
                    MessageBoxImage.Error);
            }
            catch (System.UnauthorizedAccessException ex)
            {
                // Нет прав доступа
                InitializeEmptyData();
                ShowError("Ошибка доступа к файлу",
                    $"Нет прав доступа к файлу данных.\n\n" +
                    $"Приложение будет работать без сохранения данных.\n\n" +
                    $"Проверьте права доступа к файлу bank_data.json.",
                    MessageBoxImage.Error);
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                // Ошибка десериализации - файл поврежден
                InitializeEmptyData();
                ShowError("Ошибка чтения данных",
                    $"Файл данных поврежден или имеет неверный формат.\n\n" +
                    $"Приложение будет работать с пустыми данными.\n\n" +
                    $"Рекомендуется восстановить файл из резервной копии или удалить поврежденный файл.",
                    MessageBoxImage.Error);
            }
            catch (System.IO.IOException ex)
            {
                // Общая ошибка ввода-вывода
                InitializeEmptyData();
                ShowError("Ошибка работы с файлом",
                    $"Произошла ошибка при работе с файлом данных:\n\n{ex.Message}\n\n" +
                    $"Приложение будет работать без сохранения данных.",
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Неожиданная ошибка
                InitializeEmptyData();
                ShowError("Неожиданная ошибка",
                    $"Произошла неожиданная ошибка при загрузке данных:\n\n{ex.Message}\n\n" +
                    $"Приложение будет работать с пустыми данными.",
                    MessageBoxImage.Error);
            }
        }

        // Метод инициализации пустых данных (fallback-режим)
        private void InitializeEmptyData()
        {
            _bankData = new BankData();
            _clients = new List<Client>();
            _accounts = new List<BankAccount>();
        }

        // Метод валидации данных банка
        private bool ValidateBankData(BankData data)
        {
            if (data == null)
            {
                return false;
            }

            // Проверяем, что списки инициализированы
            if (data.Clients == null || data.Accounts == null)
            {
                return false;
            }

            // Валидация клиентов
            foreach (var client in data.Clients)
            {
                if (client == null)
                {
                    return false;
                }

                // Проверяем обязательные поля клиента
                if (string.IsNullOrWhiteSpace(client.FullName) ||
                    string.IsNullOrWhiteSpace(client.PassportData))
                {
                    return false;
                }
            }

            // Валидация счетов
            foreach (var account in data.Accounts)
            {
                if (account == null)
                {
                    return false;
                }

                // Проверяем обязательные поля счета
                if (string.IsNullOrWhiteSpace(account.AccountNumber))
                {
                    return false;
                }

                // Проверяем корректность баланса (может быть отрицательным для банкротства)
                // Но проверяем на разумные пределы
                if (account.Balance < -1000000 || account.Balance > 1000000000)
                {
                    return false;
                }

                // Проверяем корректность срока вклада
                if (account.DepositTermDays < 0 || account.DepositTermDays > 36500) // Максимум 100 лет
                {
                    return false;
                }
            }

            return true;
        }

        // Метод валидации данных клиента
        private bool ValidateClientData()
        {
            if (_currentClientAccount == null)
            {
                return false;
            }

            // Проверяем, что клиент существует в базе
            if (_clients == null || _clients.Count == 0)
            {
                return false;
            }

            Client client = _clients.FirstOrDefault(c =>
                c.PassportData.Trim() == _currentClientAccount.PassportData.Trim());

            if (client == null)
            {
                return false;
            }

            // Проверяем наличие счетов у клиента
            if (_clientAccounts == null || _clientAccounts.Count == 0)
            {
                // Это не ошибка, просто у клиента нет счетов
                return true;
            }

            // Проверяем корректность данных счетов
            foreach (var account in _clientAccounts)
            {
                if (account == null)
                {
                    return false;
                }

                // Проверяем, что счет действительно принадлежит клиенту
                if (string.IsNullOrEmpty(account.OwnerPassportData) ||
                    account.OwnerPassportData.Trim() != _currentClientAccount.PassportData.Trim())
                {
                    return false;
                }
            }

            return true;
        }

        // Метод отображения ошибки с улучшенным сообщением
        private void ShowError(string title, string message, MessageBoxImage icon)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        // Метод фильтрации счетов по паспортным данным текущего клиента
        private void FilterClientAccounts()
        {
            if (_currentClientAccount == null || _accounts == null)
            {
                _clientAccounts = new List<BankAccount>();
                _filteredAccounts = new List<BankAccount>();
                return;
            }

            // Валидация: проверяем, что паспортные данные не пустые
            if (string.IsNullOrWhiteSpace(_currentClientAccount.PassportData))
            {
                _clientAccounts = new List<BankAccount>();
                _filteredAccounts = new List<BankAccount>();
                ShowError("Ошибка данных",
                    "Паспортные данные клиента не указаны. Невозможно загрузить счета.",
                    MessageBoxImage.Warning);
                return;
            }

            // Фильтруем счета по паспортным данным авторизованного клиента
            _clientAccounts = _accounts.Where(a => 
                !string.IsNullOrEmpty(a.OwnerPassportData) && 
                a.OwnerPassportData.Trim() == _currentClientAccount.PassportData.Trim())
                .ToList();

            // Проверяем наличие счетов у клиента
            if (_clientAccounts.Count == 0 && _currentClientAccount != null)
            {
                // Это не ошибка, просто у клиента нет счетов
                // Можно показать информационное сообщение, но не ошибку
            }
        }

        // Метод загрузки всех транзакций из счетов клиента
        private void LoadTransactions()
        {
            _allTransactions = new List<Transaction>();

            if (_clientAccounts == null || _clientAccounts.Count == 0)
            {
                return;
            }

            try
            {
                // Собираем все транзакции из всех счетов клиента
                foreach (var account in _clientAccounts)
                {
                    if (account == null)
                    {
                        continue; // Пропускаем null-счета
                    }

                    if (account.TransactionHistory != null && account.TransactionHistory.Count > 0)
                    {
                        // Валидация транзакций перед добавлением
                        foreach (var transaction in account.TransactionHistory)
                        {
                            if (transaction != null && ValidateTransaction(transaction))
                            {
                                _allTransactions.Add(transaction);
                            }
                        }
                    }
                }

                // Сортируем по дате (от новых к старым)
                _allTransactions = _allTransactions.OrderByDescending(t => t.TransactionDate).ToList();
            }
            catch (Exception ex)
            {
                // Ошибка при загрузке транзакций - не критично, продолжаем работу
                _allTransactions = new List<Transaction>();
                // Не показываем ошибку пользователю, так как это не критично
            }
        }

        // Метод валидации транзакции
        private bool ValidateTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                return false;
            }

            // Проверяем обязательные поля
            if (string.IsNullOrWhiteSpace(transaction.AccountNumber))
            {
                return false;
            }

            // Проверяем корректность суммы
            if (transaction.Amount < 0 || transaction.Amount > 1000000000) // Максимум 1 миллиард
            {
                return false;
            }

            // Проверяем корректность даты (не должна быть в будущем более чем на 1 день)
            if (transaction.TransactionDate > DateTime.Now.AddDays(1))
            {
                return false;
            }

            return true;
        }

        // Метод фильтрации транзакций
        private void FilterTransactions()
        {
            try
            {
                if (_allTransactions == null)
                {
                    _filteredTransactions = new List<Transaction>();
                    return;
                }

                // Начинаем с полного списка транзакций
                IEnumerable<Transaction> filtered = _allTransactions.Where(t => t != null); // Фильтруем null-транзакции

                // Фильтрация по типу операции
                if (cmbTransactionTypeFilter != null && cmbTransactionTypeFilter.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                {
                    string typeTag = selectedItem.Tag?.ToString();
                    if (!string.IsNullOrEmpty(typeTag) && typeTag != "All")
                    {
                        TransactionType filterType;
                        if (Enum.TryParse(typeTag, out filterType))
                        {
                            filtered = filtered.Where(t => t != null && t.TransactionType == filterType);
                        }
                    }
                }

                // Фильтрация по дате от
                if (dpDateFrom != null && dpDateFrom.SelectedDate.HasValue)
                {
                    DateTime dateFrom = dpDateFrom.SelectedDate.Value.Date;
                    filtered = filtered.Where(t => t != null && t.TransactionDate.Date >= dateFrom);
                }

                // Фильтрация по дате до
                if (dpDateTo != null && dpDateTo.SelectedDate.HasValue)
                {
                    DateTime dateTo = dpDateTo.SelectedDate.Value.Date.AddDays(1); // Включаем весь день
                    filtered = filtered.Where(t => t != null && t.TransactionDate < dateTo);
                }

                // Фильтрация по сумме от
                if (txtAmountFrom != null && !string.IsNullOrWhiteSpace(txtAmountFrom.Text))
                {
                    if (decimal.TryParse(txtAmountFrom.Text, out decimal amountFrom))
                    {
                        filtered = filtered.Where(t => t != null && t.Amount >= amountFrom);
                    }
                }

                // Фильтрация по сумме до
                if (txtAmountTo != null && !string.IsNullOrWhiteSpace(txtAmountTo.Text))
                {
                    if (decimal.TryParse(txtAmountTo.Text, out decimal amountTo))
                    {
                        filtered = filtered.Where(t => t != null && t.Amount <= amountTo);
                    }
                }

                // Сохраняем отфильтрованный список
                _filteredTransactions = filtered.ToList();
            }
            catch (Exception ex)
            {
                // Ошибка при фильтрации - используем полный список
                _filteredTransactions = _allTransactions ?? new List<Transaction>();
                // Не показываем ошибку пользователю, так как это не критично
            }
        }

        // Метод обновления DataGrid с транзакциями
        private void UpdateTransactionsGrid()
        {
            try
            {
                if (dgTransactions != null)
                {
                    // Создаем список оберток для привязки, фильтруя null-транзакции
                    var transactionViews = _filteredTransactions?
                        .Where(t => t != null)
                        .Select(t => new TransactionView(t))
                        .ToList() ?? new List<TransactionView>();
                    
                    dgTransactions.ItemsSource = transactionViews;
                }
            }
            catch (Exception)
            {
                // Ошибка при обновлении - очищаем DataGrid
                if (dgTransactions != null)
                {
                    dgTransactions.ItemsSource = new List<TransactionView>();
                }
                // Не показываем ошибку пользователю, так как это не критично
            }
        }

        // Метод обработки изменения фильтра по типу операции
        private void cmbTransactionTypeFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterTransactions();
            UpdateTransactionsGrid();
        }

        // Метод обработки изменения даты от
        private void dpDateFrom_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterTransactions();
            UpdateTransactionsGrid();
        }

        // Метод обработки изменения даты до
        private void dpDateTo_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterTransactions();
            UpdateTransactionsGrid();
        }

        // Метод обработки изменения суммы от
        private void txtAmountFrom_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterTransactions();
            UpdateTransactionsGrid();
        }

        // Метод обработки изменения суммы до
        private void txtAmountTo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterTransactions();
            UpdateTransactionsGrid();
        }

        // Класс-обертка для отображения транзакции в DataGrid
        private class TransactionView
        {
            private Transaction _transaction;

            public TransactionView(Transaction transaction)
            {
                _transaction = transaction;
            }

            public DateTime TransactionDate => _transaction.TransactionDate;
            public string TransactionTypeString => _transaction.GetTransactionTypeString();
            public decimal Amount => _transaction.Amount;
            public string Description => _transaction.Description;
            public string TargetAccountNumber => _transaction.TargetAccountNumber ?? "-";
        }

        // Метод обновления UI
        private void UpdateUI()
        {
            // Обновляем DataGrid со счетами
            UpdateAccountsGrid();

            // Обновляем информацию о клиенте
            UpdateClientInfo();

            // Обновляем статистику
            UpdateStatistics();

            // Обновляем историю операций
            UpdateTransactionsGrid();

            // Обновляем форму перевода
            UpdateTransferForm();
        }

        // Метод обновления формы перевода
        private void UpdateTransferForm()
        {
            try
            {
                if (cmbTransferFrom != null)
                {
                    // Загружаем счета клиента в ComboBox
                    var accountViews = _clientAccounts?
                        .Where(a => a != null && a.Status == AccountStatus.Open)
                        .Select(a => new AccountView(a))
                        .ToList() ?? new List<AccountView>();

                    cmbTransferFrom.ItemsSource = accountViews;

                    // Выбираем первый счет по умолчанию
                    if (accountViews.Count > 0 && cmbTransferFrom.SelectedIndex == -1)
                    {
                        cmbTransferFrom.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception)
            {
                // Ошибка при обновлении формы - не критично
            }
        }

        // Метод обновления DataGrid со счетами
        private void UpdateAccountsGrid()
        {
            try
            {
                if (dgAccounts != null)
                {
                    // Создаем список оберток для привязки из отфильтрованных счетов, фильтруя null-счета
                    var accountViews = _filteredAccounts?
                        .Where(a => a != null)
                        .Select(a => new AccountView(a))
                        .ToList() ?? new List<AccountView>();
                    
                    dgAccounts.ItemsSource = accountViews;
                }
            }
            catch (Exception)
            {
                // Ошибка при обновлении - очищаем DataGrid
                if (dgAccounts != null)
                {
                    dgAccounts.ItemsSource = new List<AccountView>();
                }
                // Не показываем ошибку пользователю, так как это не критично
            }
        }

        // Метод фильтрации счетов
        private void FilterAccounts()
        {
            try
            {
                if (_clientAccounts == null)
                {
                    _filteredAccounts = new List<BankAccount>();
                    return;
                }

                // Начинаем с полного списка счетов клиента
                IEnumerable<BankAccount> filtered = _clientAccounts.Where(a => a != null); // Фильтруем null-счета

                // Фильтрация по статусу
                if (cmbStatusFilter != null && cmbStatusFilter.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
                {
                    string statusTag = selectedItem.Tag?.ToString();
                    if (!string.IsNullOrEmpty(statusTag) && statusTag != "All")
                    {
                        AccountStatus filterStatus;
                        if (Enum.TryParse(statusTag, out filterStatus))
                        {
                            filtered = filtered.Where(a => a != null && a.Status == filterStatus);
                        }
                    }
                }

                // Поиск по номеру счета (частичное совпадение)
                if (txtSearchAccount != null && !string.IsNullOrWhiteSpace(txtSearchAccount.Text))
                {
                    string searchText = txtSearchAccount.Text.Trim().ToLower();
                    filtered = filtered.Where(a => 
                        a != null &&
                        !string.IsNullOrEmpty(a.AccountNumber) && 
                        a.AccountNumber.ToLower().Contains(searchText));
                }

                // Сохраняем отфильтрованный список
                _filteredAccounts = filtered.ToList();
            }
            catch (Exception ex)
            {
                // Ошибка при фильтрации - используем полный список
                _filteredAccounts = _clientAccounts ?? new List<BankAccount>();
                // Не показываем ошибку пользователю, так как это не критично
            }
        }

        // Метод обновления информации о клиенте
        private void UpdateClientInfo()
        {
            if (_currentClientAccount == null)
            {
                // Если клиент не авторизован, скрываем информацию
                if (txtFullName != null) txtFullName.Text = "Не авторизован";
                if (txtPassportData != null) txtPassportData.Text = "-";
                if (txtDateOfBirth != null) txtDateOfBirth.Text = "-";
                if (btnLogout != null) btnLogout.Visibility = Visibility.Collapsed;
                return;
            }

            // Валидация данных клиента
            if (!ValidateClientData())
            {
                // Данные клиента некорректны
                if (txtFullName != null) txtFullName.Text = "Ошибка загрузки данных";
                if (txtPassportData != null) txtPassportData.Text = _currentClientAccount.PassportData ?? "-";
                if (txtDateOfBirth != null) txtDateOfBirth.Text = "Не удалось загрузить";
                
                // Показываем предупреждение только один раз
                if (!_validationWarningShown)
                {
                    ShowError("Предупреждение о данных",
                        "Не удалось загрузить полные данные о клиенте из базы.\n\n" +
                        "Возможно, данные клиента отсутствуют в системе или файл данных поврежден.",
                        MessageBoxImage.Warning);
                    _validationWarningShown = true;
                }
                
                if (btnLogout != null) btnLogout.Visibility = Visibility.Visible;
                return;
            }

            // Отображаем информацию из ClientAccount
            if (txtFullName != null) txtFullName.Text = _currentClientAccount.FullName;
            if (txtPassportData != null) txtPassportData.Text = _currentClientAccount.PassportData;

            // Ищем клиента в базе для получения даты рождения
            Client client = _clients?.FirstOrDefault(c => 
                c.PassportData.Trim() == _currentClientAccount.PassportData.Trim());

            if (client != null && client.DateOfBirth != DateTime.MinValue)
            {
                if (txtDateOfBirth != null) 
                    txtDateOfBirth.Text = client.DateOfBirth.ToString("dd.MM.yyyy");
            }
            else
            {
                if (txtDateOfBirth != null) txtDateOfBirth.Text = "Не указана";
            }

            // Показываем кнопку выхода
            if (btnLogout != null) btnLogout.Visibility = Visibility.Visible;
        }

        // Метод обработки кнопки "Обновить"
        private void btnRefreshAccounts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сбрасываем флаг предупреждения для повторной проверки
                _validationWarningShown = false;

                LoadData();
                FilterClientAccounts();
                FilterAccounts();
                LoadTransactions();
                FilterTransactions();
                UpdateAccountsGrid();
                UpdateStatistics();
                UpdateTransactionsGrid();
                UpdateClientInfo();
            }
            catch (Exception ex)
            {
                ShowError("Ошибка обновления",
                    $"Произошла ошибка при обновлении данных:\n\n{ex.Message}\n\n" +
                    $"Попробуйте закрыть и открыть окно заново.",
                    MessageBoxImage.Error);
            }
        }

        // Метод обработки изменения фильтра по статусу
        private void cmbStatusFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterAccounts();
            UpdateAccountsGrid();
            UpdateStatistics();
        }

        // Метод обработки изменения текста поиска
        private void txtSearchAccount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterAccounts();
            UpdateAccountsGrid();
            UpdateStatistics();
        }

        // Метод обработки кнопки "Сбросить фильтры"
        private void btnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем фильтр по статусу
            if (cmbStatusFilter != null)
            {
                cmbStatusFilter.SelectedIndex = 0; // "Все"
            }

            // Очищаем поле поиска
            if (txtSearchAccount != null)
            {
                txtSearchAccount.Text = string.Empty;
            }

            // Применяем фильтры (которые теперь сброшены)
            FilterAccounts();
            UpdateAccountsGrid();
            UpdateStatistics();
        }

        // Класс для хранения статистических данных
        private class AccountStatistics
        {
            public int TotalAccounts { get; set; }
            public decimal TotalBalance { get; set; }
            public int OpenAccounts { get; set; }
            public int ClosedAccounts { get; set; }
            public decimal AverageBalance { get; set; }
        }

        // Метод расчета статистики
        private AccountStatistics CalculateStatistics()
        {
            var stats = new AccountStatistics();

            try
            {
                if (_clientAccounts == null || _clientAccounts.Count == 0)
                {
                    return stats;
                }

                // Фильтруем null-счета перед расчетом
                var validAccounts = _clientAccounts.Where(a => a != null).ToList();

                if (validAccounts.Count == 0)
                {
                    return stats;
                }

                // Общее количество счетов
                stats.TotalAccounts = validAccounts.Count;

                // Общий баланс по всем счетам (с проверкой на переполнение)
                try
                {
                    stats.TotalBalance = validAccounts.Sum(a => a.Balance);
                }
                catch (OverflowException)
                {
                    // Если сумма слишком большая, устанавливаем максимальное значение
                    stats.TotalBalance = decimal.MaxValue;
                }

                // Количество открытых счетов
                stats.OpenAccounts = validAccounts.Count(a => a.Status == AccountStatus.Open);

                // Количество закрытых счетов
                stats.ClosedAccounts = validAccounts.Count(a => a.Status == AccountStatus.Closed);

                // Средний баланс по счетам
                if (stats.TotalAccounts > 0)
                {
                    try
                    {
                        stats.AverageBalance = stats.TotalBalance / stats.TotalAccounts;
                    }
                    catch (DivideByZeroException)
                    {
                        stats.AverageBalance = 0;
                    }
                }
                else
                {
                    stats.AverageBalance = 0;
                }
            }
            catch (Exception)
            {
                // При любой ошибке возвращаем пустую статистику
                // Не показываем ошибку пользователю, так как это не критично
            }

            return stats;
        }

        // Метод обновления отображения статистики
        private void UpdateStatistics()
        {
            try
            {
                AccountStatistics stats = CalculateStatistics();

                // Общее количество счетов
                if (txtTotalAccounts != null)
                {
                    txtTotalAccounts.Text = stats.TotalAccounts.ToString();
                }

                // Общий баланс
                if (txtTotalBalance != null)
                {
                    if (stats.TotalBalance == decimal.MaxValue)
                    {
                        txtTotalBalance.Text = "Слишком большое значение";
                    }
                    else
                    {
                        txtTotalBalance.Text = $"{stats.TotalBalance:F2} ₽";
                    }
                }

                // Количество открытых счетов
                if (txtOpenAccounts != null)
                {
                    txtOpenAccounts.Text = stats.OpenAccounts.ToString();
                }

                // Количество закрытых счетов
                if (txtClosedAccounts != null)
                {
                    txtClosedAccounts.Text = stats.ClosedAccounts.ToString();
                }

                // Средний баланс
                if (txtAverageBalance != null)
                {
                    if (stats.AverageBalance == decimal.MaxValue)
                    {
                        txtAverageBalance.Text = "Слишком большое значение";
                    }
                    else
                    {
                        txtAverageBalance.Text = $"{stats.AverageBalance:F2} ₽";
                    }
                }
            }
            catch (Exception)
            {
                // Ошибка при обновлении статистики - показываем нули
                if (txtTotalAccounts != null) txtTotalAccounts.Text = "0";
                if (txtTotalBalance != null) txtTotalBalance.Text = "0.00 ₽";
                if (txtOpenAccounts != null) txtOpenAccounts.Text = "0";
                if (txtClosedAccounts != null) txtClosedAccounts.Text = "0";
                if (txtAverageBalance != null) txtAverageBalance.Text = "0.00 ₽";
                // Не показываем ошибку пользователю, так как это не критично
            }
        }

        // Метод обработки кнопки "Выйти из аккаунта"
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите выйти из аккаунта?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _isBackButtonClicked = true;
                this.Close();
            }
        }

        // Метод обработки двойного клика на счет в DataGrid
        private void dgAccounts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgAccounts.SelectedItem is AccountView accountView)
            {
                // Валидация: проверяем, что accountView не null
                if (accountView == null || string.IsNullOrEmpty(accountView.AccountNumber))
                {
                    ShowError("Ошибка выбора",
                        "Не удалось определить выбранный счет. Попробуйте выбрать счет еще раз.",
                        MessageBoxImage.Warning);
                    return;
                }

                // Находим оригинальный BankAccount из списка
                BankAccount account = _clientAccounts?.FirstOrDefault(a => 
                    a != null && a.AccountNumber == accountView.AccountNumber);

                if (account == null)
                {
                    ShowError("Ошибка данных",
                        "Выбранный счет не найден в базе данных. Возможно, данные были изменены.\n\n" +
                        "Попробуйте обновить список счетов.",
                        MessageBoxImage.Warning);
                    return;
                }

                // Валидация данных счета перед открытием окна
                if (string.IsNullOrEmpty(account.AccountNumber))
                {
                    ShowError("Ошибка данных",
                        "Данные выбранного счета некорректны. Номер счета не указан.",
                        MessageBoxImage.Error);
                    return;
                }

                try
                {
                    AccountDetailsWindow detailsWindow = new AccountDetailsWindow(account);
                    detailsWindow.ShowDialog();
                }
                catch (ArgumentNullException)
                {
                    ShowError("Ошибка открытия окна",
                        "Не удалось открыть окно детальной информации: данные счета отсутствуют.",
                        MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    ShowError("Ошибка открытия окна",
                        $"Произошла ошибка при открытии окна детальной информации:\n\n{ex.Message}",
                        MessageBoxImage.Error);
                }
            }
        }

        // Класс-обертка для отображения счета в DataGrid
        private class AccountView
        {
            private BankAccount _account;

            public AccountView(BankAccount account)
            {
                _account = account;
            }

            public string AccountNumber => _account.AccountNumber;
            public DateTime OpeningDate => _account.OpeningDate;
            public decimal Balance => _account.Balance;
            public string Status => GetStatusString(_account.Status);
            public System.Windows.Media.Brush StatusColor => GetStatusColor(_account.Status);
            public int DepositTermDays => _account.DepositTermDays;
            public DateTime DepositEndDate => _account.GetDepositEndDate();

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

            private System.Windows.Media.Brush GetStatusColor(AccountStatus status)
            {
                switch (status)
                {
                    case AccountStatus.Open:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 150, 0)); // Зеленый
                    case AccountStatus.Closed:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128)); // Серый
                    case AccountStatus.Bankrupt:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(200, 0, 0)); // Красный
                    default:
                        return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(128, 128, 128)); // Серый по умолчанию
                }
            }
        }

        // Метод обработки выбора счета-источника
        private void cmbTransferFrom_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbTransferFrom?.SelectedItem is AccountView accountView && accountView != null)
                {
                    // Показываем информацию о счете-источнике
                    if (borderSourceAccountInfo != null)
                    {
                        borderSourceAccountInfo.Visibility = Visibility.Visible;
                    }

                    if (txtSourceAccountInfo != null)
                    {
                        txtSourceAccountInfo.Text = $"Номер счета: {accountView.AccountNumber}\n" +
                                                    $"Баланс: {accountView.Balance:F2} ₽\n" +
                                                    $"Статус: {accountView.Status}";
                    }
                }
                else
                {
                    if (borderSourceAccountInfo != null)
                    {
                        borderSourceAccountInfo.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception)
            {
                // Ошибка при обновлении - не критично
            }
        }

        // Метод обработки кнопки "Выполнить перевод"
        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Скрываем предыдущий результат
                if (borderTransferResult != null)
                {
                    borderTransferResult.Visibility = Visibility.Collapsed;
                }

                // Валидация: проверяем выбор счета-источника
                if (cmbTransferFrom?.SelectedItem == null)
                {
                    ShowTransferResult("Ошибка: выберите счет-источник", false);
                    MessageBox.Show("Пожалуйста, выберите счет, с которого будет выполнен перевод.",
                        "Ошибка валидации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                AccountView sourceAccountView = cmbTransferFrom.SelectedItem as AccountView;
                if (sourceAccountView == null)
                {
                    ShowTransferResult("Ошибка: не удалось определить счет-источник", false);
                    return;
                }

                // Находим оригинальный BankAccount
                BankAccount sourceAccount = _clientAccounts?.FirstOrDefault(a =>
                    a != null && a.AccountNumber == sourceAccountView.AccountNumber);

                if (sourceAccount == null)
                {
                    ShowTransferResult("Ошибка: счет-источник не найден в базе данных", false);
                    return;
                }

                // Валидация: проверяем статус счета-источника
                if (sourceAccount.Status != AccountStatus.Open)
                {
                    ShowTransferResult($"Ошибка: счет-источник имеет статус '{GetStatusString(sourceAccount.Status)}'. Перевод возможен только с открытых счетов.", false);
                    MessageBox.Show($"Счет-источник не открыт. Текущий статус: {GetStatusString(sourceAccount.Status)}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Валидация: проверяем номер целевого счета
                if (txtTransferTo == null || string.IsNullOrWhiteSpace(txtTransferTo.Text))
                {
                    ShowTransferResult("Ошибка: введите номер целевого счета", false);
                    MessageBox.Show("Пожалуйста, введите номер счета получателя.",
                        "Ошибка валидации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                string targetAccountNumber = txtTransferTo.Text.Trim();

                // Валидация: проверяем, что целевой счет не совпадает с источником
                if (targetAccountNumber == sourceAccount.AccountNumber)
                {
                    ShowTransferResult("Ошибка: нельзя переводить средства на тот же счет", false);
                    MessageBox.Show("Нельзя переводить средства на тот же счет!",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Ищем целевой счет в базе данных
                BankAccount targetAccount = _accounts?.FirstOrDefault(a =>
                    a != null && a.AccountNumber == targetAccountNumber);

                if (targetAccount == null)
                {
                    ShowTransferResult($"Ошибка: счет с номером '{targetAccountNumber}' не найден в системе", false);
                    MessageBox.Show($"Счет с номером '{targetAccountNumber}' не найден в системе.\n\n" +
                                  "Проверьте правильность введенного номера счета.",
                        "Счет не найден",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Валидация: проверяем статус целевого счета
                if (targetAccount.Status != AccountStatus.Open)
                {
                    ShowTransferResult($"Ошибка: целевой счет имеет статус '{GetStatusString(targetAccount.Status)}'. Перевод возможен только на открытые счета.", false);
                    MessageBox.Show($"Целевой счет не открыт. Текущий статус: {GetStatusString(targetAccount.Status)}",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Валидация: проверяем сумму перевода
                if (txtTransferAmount == null || string.IsNullOrWhiteSpace(txtTransferAmount.Text))
                {
                    ShowTransferResult("Ошибка: введите сумму перевода", false);
                    MessageBox.Show("Пожалуйста, введите сумму для перевода.",
                        "Ошибка валидации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                string amountText = txtTransferAmount.Text.Replace(",", ".");
                if (!decimal.TryParse(amountText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
                {
                    ShowTransferResult("Ошибка: введите корректную положительную сумму", false);
                    MessageBox.Show("Пожалуйста, введите корректную положительную сумму для перевода.",
                        "Ошибка валидации",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Валидация: проверяем достаточность средств
                if (sourceAccount.Balance < amount)
                {
                    ShowTransferResult($"Ошибка: недостаточно средств на счете. Доступно: {sourceAccount.Balance:F2} ₽", false);
                    MessageBox.Show($"Недостаточно средств на счете для перевода.\n\n" +
                                  $"Доступно: {sourceAccount.Balance:F2} ₽\n" +
                                  $"Запрошено: {amount:F2} ₽",
                        "Недостаточно средств",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Подтверждение перевода
                MessageBoxResult confirmResult = MessageBox.Show(
                    $"Вы уверены, что хотите перевести {amount:F2} ₽\n\n" +
                    $"Со счета: {sourceAccount.AccountNumber}\n" +
                    $"На счет: {targetAccount.AccountNumber}?",
                    "Подтверждение перевода",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirmResult != MessageBoxResult.Yes)
                {
                    return;
                }

                // Выполняем перевод
                bool transferResult = sourceAccount.Transfer(targetAccount, amount);

                if (transferResult)
                {
                    // Сохраняем данные в JSON
                    SaveData();

                    // Обновляем UI
                    FilterClientAccounts();
                    FilterAccounts();
                    LoadTransactions();
                    FilterTransactions();
                    UpdateAccountsGrid();
                    UpdateStatistics();
                    UpdateTransactionsGrid();
                    UpdateTransferForm();

                    // Очищаем поля
                    if (txtTransferAmount != null)
                    {
                        txtTransferAmount.Text = string.Empty;
                    }

                    // Показываем успешный результат
                    string successMessage = $"Перевод выполнен успешно!\n\n" +
                                          $"Со счета: {sourceAccount.AccountNumber}\n" +
                                          $"На счет: {targetAccount.AccountNumber}\n" +
                                          $"Сумма: {amount:F2} ₽";
                    ShowTransferResult(successMessage, true);

                    MessageBox.Show(successMessage,
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    // Определяем причину ошибки
                    string errorMessage = "Ошибка при переводе средств";
                    if (sourceAccount.Status != AccountStatus.Open)
                    {
                        errorMessage = "Счет-источник не открыт!";
                    }
                    else if (targetAccount.Status != AccountStatus.Open)
                    {
                        errorMessage = "Целевой счет не открыт!";
                    }
                    else if (sourceAccount.Balance < amount)
                    {
                        errorMessage = "Недостаточно средств на счете-источнике!";
                    }
                    else
                    {
                        errorMessage = "Неизвестная ошибка при переводе!";
                    }

                    ShowTransferResult(errorMessage, false);
                    MessageBox.Show(errorMessage,
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                ShowTransferResult($"Ошибка при выполнении перевода: {ex.Message}", false);
                ShowError("Ошибка перевода",
                    $"Произошла ошибка при выполнении перевода:\n\n{ex.Message}",
                    MessageBoxImage.Error);
            }
        }

        // Метод отображения результата перевода
        private void ShowTransferResult(string message, bool isSuccess)
        {
            if (borderTransferResult != null)
            {
                borderTransferResult.Visibility = Visibility.Visible;
            }

            if (txtTransferResult != null)
            {
                txtTransferResult.Text = message;
                if (isSuccess)
                {
                    txtTransferResult.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(0, 150, 0)); // Зеленый
                }
                else
                {
                    txtTransferResult.Foreground = new System.Windows.Media.SolidColorBrush(
                        System.Windows.Media.Color.FromRgb(200, 0, 0)); // Красный
                }
            }
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

        // Метод обработки ввода текста в поле суммы перевода (только числа и точка/запятая)
        private void txtTransferAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры, точку и запятую
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Метод сохранения данных в JSON
        private void SaveData()
        {
            try
            {
                if (_dataService == null || _accounts == null || _clients == null)
                {
                    return;
                }

                // Обновляем OwnerPassportData для всех счетов перед сохранением
                foreach (var account in _accounts)
                {
                    if (account != null && account.Owner != null)
                    {
                        account.OwnerPassportData = account.Owner.PassportData;
                    }
                }

                // Создаем объект BankData для сохранения
                BankData dataToSave = new BankData(_clients, _accounts);

                // Сохраняем в JSON
                string filePath = _dataService.GetDataFilePath();
                _dataService.SaveToJson(dataToSave, filePath);
            }
            catch (Exception ex)
            {
                ShowError("Ошибка сохранения",
                    $"Произошла ошибка при сохранении данных:\n\n{ex.Message}\n\n" +
                    $"Изменения могут быть потеряны при закрытии приложения.",
                    MessageBoxImage.Warning);
            }
        }
    }
}

