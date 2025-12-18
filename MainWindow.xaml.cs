using System;
using System.ComponentModel;
using System.Windows;
using SimpleBank.View;

namespace SimpleBank
{
    public partial class MainWindow : Window
    {
        // Флаг нажатия кнопки "Выход"
        private bool _isExitButtonClicked = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработчик кнопки "Клиент банка"
        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserInformationWindow userInfoWindow = new UserInformationWindow();
                this.Hide();
                userInfoWindow.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show();
            }
        }

        // Обработчик кнопки "Сотрудник банка"
        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AuthorizationWindow authWindow = new AuthorizationWindow("Employee");
                this.Hide();
                authWindow.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show();
            }
        }

        // Обработчик кнопки "Системный администратор"
        private void btnAdministrator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AuthorizationWindow authWindow = new AuthorizationWindow("Administrator");
                this.Hide();
                authWindow.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show();
            }
        }

        // Обработчик кнопки "Тестирование классов"
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TestWindow testWindow = new TestWindow();
                this.Hide();
                testWindow.ShowDialog();
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна тестирования: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show();
            }
        }

        // Обработчик кнопки "Выход"
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            _isExitButtonClicked = true;
            
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                _isExitButtonClicked = false;
            }
        }

        // Обработчик закрытия окна
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_isExitButtonClicked)
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
