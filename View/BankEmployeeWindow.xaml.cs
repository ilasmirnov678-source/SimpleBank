using System;
using System.ComponentModel;
using System.Windows;

namespace SimpleBank.View
{
    public partial class BankEmployeeWindow : Window
    {
        // Флаг для отслеживания нажатия кнопки "Назад" - используется для предотвращения подтверждения закрытия
        private bool _isBackButtonClicked = false;

        public BankEmployeeWindow()
        {
            InitializeComponent();
        }

        // Обработка нажатия кнопки "Назад": устанавливает флаг и закрывает окно без подтверждения
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _isBackButtonClicked = true;
            this.Close();
        }

        // Обработка события закрытия окна: если закрытие происходит не через кнопку "Назад",
        // запрашивает подтверждение закрытия. При отказе отменяет закрытие
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

