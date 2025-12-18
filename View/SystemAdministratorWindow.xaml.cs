using System;
using System.ComponentModel;
using System.Windows;

namespace SimpleBank.View
{
    /// <summary>
    /// Логика взаимодействия для SystemAdministratorWindow.xaml
    /// </summary>
    public partial class SystemAdministratorWindow : Window
    {
        public SystemAdministratorWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
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

