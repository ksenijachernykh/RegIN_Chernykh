using RegIN_Chernykh.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegIN_Chernykh.Pages
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    


    public partial class Login : Page
    {
        /// <summary>
        /// Логин введёный пользователем
        /// </summary>
        string OldLogin;

        /// <summary>
        /// Кол-во попыток ввода пароля
        /// </summary>
        int CountSetPassword = 2;

        /// <summary>
        /// Переменная отвечающая за ввод капчи
        /// </summary>
        bool IsCapture = false;

        public Login()
        {
            InitializeComponent();
            MainWindow.mainWindow.UserLogin.HandlerCorrectLogin += CorrectLogin;
            MainWindow.mainWindow.UserLogin.HandlerInCorrectLogin += InCorrectLogin;
            Capture.HandlerCorrectCapture += CorrectCapture;
        }

        /// <summary>
        /// Метод правильно введённого логина
        /// </summary>
        public void CorrectLogin()
        {
            if (OldLogin != TbLogin.Text)
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                try
                {
                    BitmapImage bling = new BitmapImage();
                    MemoryStream ms = new MemoryStream(MainWindow.mainWindow.UserLogIn.Image);
                    bling.BeginInit();
                    bling.StreamSource = ms;
                    bling.EndInit();

                    ImageSource imgSrc = bling;
                    DoubleAnimation StartAnimation = new DoubleAnimation();
                    StartAnimation.From = 1;
                    StartAnimation.To = 0;
                    StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                    StartAnimation.Completed += delegate
                    {
                        IUser.Source = imgSrc;
                        DoubleAnimation EndAnimation = new DoubleAnimation();
                        EndAnimation.From = 0;
                        EndAnimation.To = 1;
                        EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                        IUser.BeginAnimation(Image.OpacityProperty, EndAnimation);
                    };

                    IUser.BeginAnimation(Image.OpacityProperty, StartAnimation);
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp.Message);
                }

                OldLogin = TbLogin.Text;
            }
        }

        /// <summary>
        /// Метод не успешной авторизации
        /// </summary>
        public void InCorrectLogin()
        {
            if (LNameUser.Content != "")
            {
                LNameUser.Content = "";
                DoubleAnimation StartAnimation = new DoubleAnimation();
                StartAnimation.From = 1;
                StartAnimation.To = 0;
                StartAnimation.Duration = TimeSpan.FromSeconds(0.6);
                StartAnimation.Completed += delegate
                {
                    IUser.Source = new BitmapImage(new Uri("pack://application:,,./Images/ic-user.png"));
                    DoubleAnimation EndAnimation = new DoubleAnimation();
                    EndAnimation.From = 0;
                    EndAnimation.To = 1;
                    EndAnimation.Duration = TimeSpan.FromSeconds(1.2);
                    IUser.BeginAnimation(OpacityProperty, EndAnimation);
                };
                IUser.BeginAnimation(OpacityProperty, StartAnimation);
            }
            if (TbLogin.Text.Length > 0)
                SetNotification("Login is incorrect", Brushes.Red);
        }

        /// <summary>
        /// Метод успешного ввода капчи
        /// </summary>
        public void CorrectCapture()
        {
            Capture.IsEnabled = false;
            IsCapture = true;
        }

        /// <summary>
        /// Ввод пароля
        /// </summary>
        private void SetPassword(object sender, RoutedEventArgs e)
        {
            if (e.Key == Key.Enter)
                SetPassword();
        }

        /// <summary>
        /// Ввод пароля
        /// </summary>
        public void SetPassword()
        {
            if (MainWindow.mainWindow.UserLogIn.Password != String.Empty)
            {
                if (IsCapture)
                {
                    if (MainWindow.mainWindow.UserLogIn.Password == TbPassword.Password)
                    {
                        MainWindow.mainWindow.OpenPage(new Confirmation(Confirmation.TypeConfirmation.Login));
                    }
                    else { 

                        if (CountSetPassword > 0)
                        {
                            SetNotification($"Password is incorrect, {CountSetPassword} attempts left", Brushes.Red);
                            CountSetPassword--;
                        }
                        else
                        {
                            Thread TBlockAutorization = new Thread(BlockAutorization);
                            TBlockAutorization.Start();
                        }
                        SendMail.SendMessage("An attempt was made to log into your account.", MainWindow.mainWindow.UserLogIn.Login);
                    }
                }
                else
                    // Если капча не пройдена, вызываем ошибку, цвет - красный
                    SetNotification("Enter capture", Brushes.Red);
            }
        }

        /// <summary>
        /// Метод блокировки авторизации
        /// </summary>
        public void BlockAutorization()
        {
            DateTime StartBlock = DateTime.Now.AddMinutes(3);
            Dispatcher.Invoke(() =>
            {
                TbLogin.IsEnabled = false;
                TbPassword.IsEnabled = false;
                Capture.IsEnabled = false;
            });
            for (int i = 0; i < 180; i++)
            {
                TimeSpan TimeIdle = StartBlock.Subtract(DateTime.Now);
                string s_minutes = TimeIdle.Minutes.ToString();
                if (TimeIdle.Minutes < 10)
                    s_minutes = "0" + TimeIdle.Minutes;
                string s_seconds = TimeIdle.Seconds.ToString();
                if (TimeIdle.Seconds < 10)
                    s_seconds = "0" + TimeIdle.Seconds;
                Dispatcher.Invoke(() =>
                {
                    SetNotification($"Reauthorization available in: {s_minutes}:{s_seconds}", Brushes.Red);
                });
                Thread.Sleep(1000);
            }
            Dispatcher.Invoke(() =>
            {
                SetNotification("Hi, " + MainWindow.mainWindow.UserLogIn.Name, Brushes.Black);
                TbLogin.IsEnabled = true;
                TbPassword.IsEnabled = true;
                Capture.IsEnabled = true;
                Capture.CreateCapture();
                IsCapture = false;
                CountSetPassword = 2;
            });
        }

        /// <summary>
        /// Ввод логина пользователя
        /// </summary>
        private void SetLogin(object sender, KeyEventArgs e)
        {
            // При нажатии на кнопку Enter
            if (e.Key == Key.Enter)
            {
                // Вызываем метод получения данных пользователя по логину
                MainWindow.mainWindow.UserLogin.GetUserLogin(TbLogin.Text);
            }
            // Если пароль пользователя введён
            if (TbPassword.Password.Length > 0)
                // Вызываем метод ввода пароля
                SetPassword();
        }

        /// <summary>
        /// Ввод логина пользователя
        /// </summary>
        private void SetLogin(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.UserLogin.GetUserLogin(TbLogin.Text);
            if (TbPassword.Password.Length > 0)
                SetPassword();
        }

        /// <summary>
        /// Метод уведомлений пользователя
        /// </summary>
        /// <param name="Message">Сообщение которое необходимо вывести</param>
        /// <param name="_Color">Цвет сообщения</param>
        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LNameUser.Content = Message;
            LNameUser.Foreground = _Color;
        }

        /// <summary>
        /// Метод открытия страницы восстановления пароля
        /// </summary>
        private void RecoveryPassword(object sender, MouseButtonEventArgs e) =>
            MainWindow.mainWindow.OpenPage(new Recovery());

        /// <summary>
        /// Метод открытия страницы регистрации
        /// </summary>
        private void OpenBegin(object sender, MouseButtonEventArgs e) =>
            MainWindow.mainWindow.OpenPage(new Begin());
    }
}
