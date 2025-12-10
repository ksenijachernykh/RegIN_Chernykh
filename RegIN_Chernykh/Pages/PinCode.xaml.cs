using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RegIN.Pages
{
    /// <summary>
    /// Логика взаимодействия для PinCode.xaml
    /// </summary>
    public partial class PinCode : Page
    {
        public PinCode()
        {
            InitializeComponent();
            TbPinCode.Focus();
        }

        private void SetPinCode(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessPinCode();
            }
        }

        private void ProcessPinCode()
        {
            if (IsValidPinCode(TbPinCode.Password))
            {
                SetNotification("", Brushes.Black);


                if (MainWindow.mainWindow != null && MainWindow.mainWindow.UserLogin != null)
                {
                    MainWindow.mainWindow.UserLogin.PinCode = TbPinCode.Password;

                    SavePinCodeToDatabase();

                    MessageBox.Show("Пин-код подтвержден.");
                    MainWindow.mainWindow.OpenPage(new Login());
                }
            }
            else
            {
                SetNotification("Pin code must be 4 digits.", Brushes.Red);
            }
        }

        private bool IsValidPinCode(string pinCode)
        {
            return new Regex(@"^\d{4}$").IsMatch(pinCode);
        }

        private void SavePinCodeToDatabase()
        {
            try
            {
                if (!string.IsNullOrEmpty(MainWindow.mainWindow.UserLogin.Login))
                {
                    var connection = Classes.WorkingDB.OpenConnection();
                    if (Classes.WorkingDB.OpenConnection(connection))
                    {
                        string query = $"UPDATE users SET pin_code = '{TbPinCode.Password}' WHERE login = '{MainWindow.mainWindow.UserLogin.Login}'";
                        Classes.WorkingDB.Query(query, connection);
                    }
                    Classes.WorkingDB.CloseConnection(connection);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения PIN-кода: {ex.Message}");
            }
        }

        public void SetNotification(string Message, SolidColorBrush _Color)
        {
            LPinCode.Content = Message;
            LPinCode.Foreground = _Color;
        }

        private void OpenLogin(object sender, MouseButtonEventArgs e)
        {
            if (MainWindow.mainWindow != null)
            {
                MainWindow.mainWindow.OpenPage(new Login());
            }
        }
    }
}