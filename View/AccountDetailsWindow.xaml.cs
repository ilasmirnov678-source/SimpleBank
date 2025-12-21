using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using SimpleBank.Models;

namespace SimpleBank.View
{
    // Окно детальной информации о счете
    public partial class AccountDetailsWindow : Window
    {
        private bool _isBackButtonClicked = false;
        private BankAccount _account;

        public AccountDetailsWindow(BankAccount account)
        {
            InitializeComponent();
            _account = account ?? throw new ArgumentNullException(nameof(account));
            LoadAccountData();
        }

        // Метод загрузки данных счета
        private void LoadAccountData()
        {
            if (_account == null)
            {
                return;
            }

            // Номер счета
            if (txtAccountNumber != null)
            {
                txtAccountNumber.Text = _account.AccountNumber;
            }

            // Владелец (ФИО)
            if (txtOwner != null)
            {
                if (_account.Owner != null && !string.IsNullOrEmpty(_account.Owner.FullName))
                {
                    txtOwner.Text = _account.Owner.FullName;
                }
                else
                {
                    txtOwner.Text = "Не указан";
                }
            }

            // Дата открытия
            if (txtOpeningDate != null)
            {
                if (_account.OpeningDate != DateTime.MinValue)
                {
                    txtOpeningDate.Text = _account.OpeningDate.ToString("dd.MM.yyyy");
                }
                else
                {
                    txtOpeningDate.Text = "Не указана";
                }
            }

            // Баланс
            if (txtBalance != null)
            {
                txtBalance.Text = $"{_account.Balance:F2} ₽";
            }

            // Статус счета с цветовой индикацией
            UpdateStatusDisplay();

            // Срок вклада (дни)
            if (txtDepositTermDays != null)
            {
                txtDepositTermDays.Text = $"{_account.DepositTermDays} дней";
            }

            // Дата окончания вклада
            if (txtDepositEndDate != null)
            {
                DateTime endDate = _account.GetDepositEndDate();
                if (endDate != DateTime.MinValue)
                {
                    txtDepositEndDate.Text = endDate.ToString("dd.MM.yyyy");
                }
                else
                {
                    txtDepositEndDate.Text = "Не указана";
                }
            }

            // Дата закрытия (если счет закрыт)
            if (_account.Status == AccountStatus.Closed)
            {
                if (gridClosingDate != null)
                {
                    gridClosingDate.Visibility = Visibility.Visible;
                }
                if (txtClosingDate != null)
                {
                    // Если есть дата закрытия, можно добавить поле в модель
                    // Пока используем дату окончания вклада
                    DateTime endDate = _account.GetDepositEndDate();
                    if (endDate != DateTime.MinValue)
                    {
                        txtClosingDate.Text = endDate.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        txtClosingDate.Text = "Не указана";
                    }
                }
            }
        }

        // Метод обновления отображения статуса с цветовой индикацией
        private void UpdateStatusDisplay()
        {
            if (_account == null)
            {
                return;
            }

            string statusText = GetStatusString(_account.Status);
            Brush statusColor = GetStatusColor(_account.Status);

            if (txtStatus != null)
            {
                txtStatus.Text = statusText;
                txtStatus.Foreground = statusColor;
            }

            if (statusIndicator != null)
            {
                statusIndicator.Background = statusColor;
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

        // Метод получения цвета для статуса
        private Brush GetStatusColor(AccountStatus status)
        {
            switch (status)
            {
                case AccountStatus.Open:
                    return new SolidColorBrush(Color.FromRgb(0, 150, 0)); // Зеленый
                case AccountStatus.Closed:
                    return new SolidColorBrush(Color.FromRgb(128, 128, 128)); // Серый
                case AccountStatus.Bankrupt:
                    return new SolidColorBrush(Color.FromRgb(200, 0, 0)); // Красный
                default:
                    return new SolidColorBrush(Color.FromRgb(128, 128, 128)); // Серый по умолчанию
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

