using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using SimpleBank.Models;

namespace SimpleBank.View
{
    public partial class TestWindow : Window
    {
        // Флаг для отслеживания нажатия кнопки "Назад" - используется для предотвращения подтверждения закрытия
        private bool _isBackButtonClicked = false;
        // StringWriter для перехвата вывода Console.WriteLine
        private StringWriter _stringWriter;
        private TextWriter _originalConsoleOut;

        public TestWindow()
        {
            InitializeComponent();
            RunTests();
        }

        // Запуск тестов и вывод результатов в TextBox
        private void RunTests()
        {
            // Сохраняем оригинальный вывод консоли
            _originalConsoleOut = Console.Out;
            
            // Создаем StringWriter для перехвата вывода
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);

            // Запускаем тесты
            BankClassesTest.RunAllTests();

            // Восстанавливаем оригинальный вывод консоли
            Console.SetOut(_originalConsoleOut);

            // Получаем результаты тестов и выводим в TextBox
            string testResults = _stringWriter.ToString();
            txtTestResults.Text = testResults;

            // Закрываем StringWriter
            _stringWriter.Close();
        }

        // Обработка нажатия кнопки "Назад": устанавливает флаг и закрывает окно без подтверждения
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            _isBackButtonClicked = true;
            this.Close();
        }

        // Обработка события закрытия окна: если закрытие происходит не через кнопку "Назад",
        // запрашивает подтверждение закрытия окна. При отказе отменяет закрытие
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

