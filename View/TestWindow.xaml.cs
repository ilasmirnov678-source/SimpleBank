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
        // Флаг нажатия кнопки "Назад"
        private bool _isBackButtonClicked = false;
        // Перехват вывода консоли
        private StringWriter _stringWriter;
        private TextWriter _originalConsoleOut;

        public TestWindow()
        {
            InitializeComponent();
            RunTests();
        }

        // Запуск тестов
        private void RunTests()
        {
            _originalConsoleOut = Console.Out;
            _stringWriter = new StringWriter();
            Console.SetOut(_stringWriter);
            BankClassesTest.RunAllTests();
            Console.SetOut(_originalConsoleOut);
            txtTestResults.Text = _stringWriter.ToString();
            _stringWriter.Close();
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

