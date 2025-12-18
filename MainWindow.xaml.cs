using System;
using System.ComponentModel;
using System.Windows;
using SimpleBank.View;

namespace SimpleBank
{
    public partial class MainWindow : Window
    {
        // Флаг для отслеживания нажатия кнопки "Выход" - используется для предотвращения двойного подтверждения
        private bool _isExitButtonClicked = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обработка нажатия кнопки "Клиент банка": скрывает главное окно, открывает окно информации для клиента модально,
        // после закрытия возвращает главное окно. При ошибке показывает сообщение и возвращает главное окно
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

        // Обработка нажатия кнопки "Сотрудник банка": скрывает главное окно, открывает окно авторизации для сотрудника модально,
        // после закрытия возвращает главное окно. При ошибке показывает сообщение и возвращает главное окно
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

        // Обработка нажатия кнопки "Системный администратор": скрывает главное окно, открывает окно авторизации для администратора модально,
        // после закрытия возвращает главное окно. При ошибке показывает сообщение и возвращает главное окно
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

        // Обработка нажатия кнопки "Тестирование классов": скрывает главное окно, открывает окно тестирования модально,
        // после закрытия возвращает главное окно. При ошибке показывает сообщение и возвращает главное окно
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

        // Обработка нажатия кнопки "Выход": устанавливает флаг, запрашивает подтверждение выхода из приложения,
        // при подтверждении закрывает приложение, при отмене сбрасывает флаг
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

        // Обработка события закрытия окна: если закрытие происходит не через кнопку "Выход" (где уже есть подтверждение),
        // запрашивает подтверждение закрытия окна. При отказе отменяет закрытие
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
