using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace SimpleBank
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработчик кнопки "Клиент банка"
        /// Открывает окно информации для клиента
        /// </summary>
        private void btnClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создать окно информации для клиента
                View.UserInformationWindow userInfoWindow = new View.UserInformationWindow();
                
                // Скрыть главное окно
                this.Hide();
                
                // Показать окно клиента модально
                userInfoWindow.ShowDialog();
                
                // После закрытия показать главное окно
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show(); // Показать главное окно в случае ошибки
            }
        }

        /// <summary>
        /// Обработчик кнопки "Сотрудник банка"
        /// Открывает окно авторизации для сотрудника
        /// </summary>
        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создать окно авторизации для сотрудника
                View.AuthorizationWindow authWindow = new View.AuthorizationWindow("Employee");
                
                // Скрыть главное окно
                this.Hide();
                
                // Показать окно авторизации модально
                authWindow.ShowDialog();
                
                // После закрытия показать главное окно
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show(); // Показать главное окно в случае ошибки
            }
        }

        /// <summary>
        /// Обработчик кнопки "Системный администратор"
        /// Открывает окно авторизации для администратора
        /// </summary>
        private void btnAdministrator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создать окно авторизации для администратора
                View.AuthorizationWindow authWindow = new View.AuthorizationWindow("Administrator");
                
                // Скрыть главное окно
                this.Hide();
                
                // Показать окно авторизации модально
                authWindow.ShowDialog();
                
                // После закрытия показать главное окно
                this.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии окна: {ex.Message}", 
                              "Ошибка", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
                this.Show(); // Показать главное окно в случае ошибки
            }
        }

        /// <summary>
        /// Обработчик кнопки "Выход"
        /// </summary>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Обработчик события закрытия окна
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Вы уверены, что хотите закрыть окно?",
                "Подтверждение закрытия",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true; // Отменить закрытие
            }
        }
    }
}
