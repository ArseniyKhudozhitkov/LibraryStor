using Library.Data;
using Library.Models;
using LibraryStor;
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


namespace Library.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                statusText.Text = "Введите логин и пароль";
                return;
            }

            User user = AppData.Context.Authenticate(username, password);

            if (user != null)
            {
                OpenMainWindow(user);
            }
            else
            {
                statusText.Text = "Неверный логин или пароль";
            }
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)
        {
            User guest = new User { Username = "Гость", Role = "Guest" };
            OpenMainWindow(guest);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }

        private void OpenMainWindow(User user)
        {
            MainWindow mainWindow = new MainWindow(user);
            mainWindow.Show();
            this.Close();
        }
    }
}