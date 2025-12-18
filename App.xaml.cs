using System;
using System.Windows;
using System.Threading;

namespace SimpleBank
{
    public partial class App : Application
    {
        // Обработчик запуска приложения
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SplashScreen splash = new SplashScreen("Resources/logo.png");
            splash.Show(false, true);
            Thread.Sleep(500);
            splash.Close(TimeSpan.FromSeconds(0));
            
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
