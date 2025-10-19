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
using Library.Models;

namespace Library.Views
{
    public partial class MainWindow : Window
    {
        private User _currentUser;

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            InitializeUI();
            ShowBooksPage();
        }

        private void InitializeUI()
        {
            txtUserInfo.Text = $"Пользователь: {_currentUser.Username} ({_currentUser.Role})";

            switch (_currentUser.Role)
            {
                case "Admin":
                    btnManageBooks.Visibility = Visibility.Visible;
                    btnManageOrders.Visibility = Visibility.Visible;
                    btnOrders.Visibility = Visibility.Visible;
                    break;
                case "Manager":
                    btnOrders.Visibility = Visibility.Visible;
                    btnManageBooks.Visibility = Visibility.Collapsed;
                    btnManageOrders.Visibility = Visibility.Collapsed;
                    break;
                case "Client":
                case "Guest":
                    btnOrders.Visibility = Visibility.Collapsed;
                    btnManageBooks.Visibility = Visibility.Collapsed;
                    btnManageOrders.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void BooksButton_Click(object sender, RoutedEventArgs e)
        {
            ShowBooksPage();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOrdersPage();
        }

        private void ManageBooksButton_Click(object sender, RoutedEventArgs e)
        {
            ShowManageBooksPage();
        }

        private void ManageOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            ShowManageOrdersPage();
        }

        private void ShowBooksPage()
        {
            var page = new BooksPage(_currentUser);
            mainFrame.Navigate(page);
            statusText.Text = "Просмотр книг";
        }

        private void ShowOrdersPage()
        {
            var page = new OrdersPage(_currentUser);
            mainFrame.Navigate(page);
            statusText.Text = "Просмотр заказов";
        }

        private void ShowManageBooksPage()
        {
            var page = new ManageBooksPage();
            mainFrame.Navigate(page);
            statusText.Text = "Управление книгами";
        }

        private void ShowManageOrdersPage()
        {
            var page = new ManageOrdersPage();
            mainFrame.Navigate(page);
            statusText.Text = "Управление заказами";
        }
    }
}
