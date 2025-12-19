using System;
using System.ComponentModel;
using System.Windows;

namespace SimpleBank.View
{
    // Окно функционала системного администратора
    public partial class SystemAdministratorWindow : Window
    {
        private bool _isBackButtonClicked = false;

        public SystemAdministratorWindow()
        {
            InitializeComponent();
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

