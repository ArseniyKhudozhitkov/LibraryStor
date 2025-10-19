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
using Library.Data;
using Library.Views;
using System.Windows;
using System.Windows.Controls;

namespace Library.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (string.IsNullOrWhiteSpace(fullName))
            {
                statusText.Text = "Введите ФИО";
                txtFullName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                statusText.Text = "Введите логин";
                txtUsername.Focus();
                return;
            }

            if (username.Length < 3)
            {
                statusText.Text = "Логин должен содержать не менее 3 символов";
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                statusText.Text = "Введите пароль";
                txtPassword.Focus();
                return;
            }

            if (!AppData.Context.ValidatePassword(password))
            {
                statusText.Text = "Пароль должен содержать не менее 6 символов";
                txtPassword.Focus();
                return;
            }

            if (password != confirmPassword)
            {
                statusText.Text = "Пароли не совпадают";
                txtConfirmPassword.Focus();
                return;
            }

            if (!string.IsNullOrEmpty(email) && !AppData.Context.ValidateEmail(email))
            {
                statusText.Text = "Введите корректный email";
                txtEmail.Focus();
                return;
            }

            bool success = AppData.Context.RegisterUser(username, password, email, fullName);

            if (success)
            {
                MessageBox.Show($"Регистрация прошла успешно!\nДобро пожаловать, {fullName}!",
                    "Успешная регистрация", MessageBoxButton.OK, MessageBoxImage.Information);

                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
            else
            {
                statusText.Text = "Пользователь с таким логином уже существует";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                if (!AppData.Context.ValidateEmail(txtEmail.Text))
                {
                    statusText.Text = "Email имеет неверный формат";
                }
                else
                {
                    statusText.Text = "Email корректен";
                }
            }
            else
            {
                statusText.Text = "Email необязателен";
            }
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                if (!AppData.Context.ValidatePassword(txtPassword.Password))
                {
                    statusText.Text = "Пароль должен содержать не менее 6 символов";
                }
                else
                {
                    statusText.Text = "Пароль корректен";
                }
            }
        }

        private void txtConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPassword.Password) && !string.IsNullOrWhiteSpace(txtConfirmPassword.Password))
            {
                if (txtPassword.Password != txtConfirmPassword.Password)
                {
                    statusText.Text = "Пароли не совпадают";
                }
                else
                {
                    statusText.Text = "Пароли совпадают";
                }
            }
        }
    }
}
