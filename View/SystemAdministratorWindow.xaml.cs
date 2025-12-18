using System;
using System.ComponentModel;
using System.Windows;

namespace SimpleBank.View
{
    public partial class SystemAdministratorWindow : Window
    {
        // Флаг нажатия кнопки "Назад"
        private bool _isBackButtonClicked = false;

        public SystemAdministratorWindow()
        {
            InitializeComponent();
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
    }
}

