ПАПКА РЕСУРСОВ ПРОЕКТА
=====================

Эта папка предназначена для хранения графических ресурсов приложения.

НЕОБХОДИМЫЕ ФАЙЛЫ:
------------------

1. logo.png (или logo.jpg)
   - Логотип банка для отображения в окнах и на заставке
   - Рекомендуемый размер: 200x100 пикселей или больше
   - Формат: PNG (с прозрачностью) или JPG
   - BuildAction: Resource (для использования в окнах)
   - BuildAction: SplashScreen (для заставки - нужно создать копию или использовать отдельный файл)

2. icon.ico
   - Иконка приложения для всех окон
   - Размер: 16x16, 32x32, 48x48, 256x256 пикселей (multi-icon)
   - Формат: ICO
   - BuildAction: Resource

КАК ДОБАВИТЬ РЕСУРСЫ:
---------------------

1. Скопируйте файлы logo.png и icon.ico в эту папку
2. В Visual Studio:
   - Правой кнопкой на файле → Properties
   - Установите Build Action:
     * Для logo.png (заставка): SplashScreen
     * Для logo.png (в окнах): Resource
     * Для icon.ico: Resource

ИСПОЛЬЗОВАНИЕ В КОДЕ:
---------------------

Логотип в XAML:
<Image Source="pack://application:,,,/Resources/logo.png" />

Иконка окна:
Icon="pack://application:,,,/Resources/icon.ico"

Или через ресурсы:
<Image Source="{StaticResource LogoImage}" />

