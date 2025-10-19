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
using Library.Models;


namespace Library.Views
{
    public partial class ManageOrdersPage : Page
    {
        private List<OrderDisplay> _orderDisplays;

        public ManageOrdersPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            LoadBooksComboBox();
            UpdateStatus("Готов к работе");
        }

        private void LoadOrders()
        {
            _orderDisplays = new List<OrderDisplay>();

            foreach (var order in AppData.Context.Orders)
            {
                var book = AppData.Context.Books.FirstOrDefault(b => b.Id == order.BookId);
                var orderDisplay = new OrderDisplay
                {
                    Id = order.Id,
                    BookId = order.BookId,
                    BookTitle = book != null ? book.Title : "Неизвестная книга",
                    Quantity = order.Quantity,
                    OrderDate = order.OrderDate,
                    CustomerName = order.CustomerName,
                    Status = order.Status,
                    TotalPrice = (book != null ? book.Price : 0) * order.Quantity
                };
                _orderDisplays.Add(orderDisplay);
            }

            ordersGrid.ItemsSource = _orderDisplays;
        }

        private void LoadBooksComboBox()
        {
            cmbBooks.ItemsSource = AppData.Context.Books;
        }

        private void UpdateStatus(string message)
        {
            statusText.Text = message;
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is OrderDisplay orderDisplay)
            {
                var order = AppData.Context.Orders.FirstOrDefault(o => o.Id == orderDisplay.Id);

                if (order != null)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ #{order.Id}?",
                        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        AppData.Context.Orders.Remove(order);
                        AppData.SaveAllData();
                        LoadOrders();
                        UpdateStatus($"Заказ #{order.Id} успешно удален");
                    }
                }
            }
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cmbBooks.SelectedItem == null)
                {
                    MessageBox.Show("Выберите книгу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedBook = cmbBooks.SelectedItem as Book;
                if (selectedBook == null)
                {
                    MessageBox.Show("Выберите книгу", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
                {
                    MessageBox.Show("Введите имя клиента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(txtOrderQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int newId = AppData.Context.Orders.Count > 0 ? AppData.Context.Orders.Max(o => o.Id) + 1 : 1;

                var newOrder = new Order
                {
                    Id = newId,
                    BookId = selectedBook.Id,
                    Quantity = quantity,
                    OrderDate = DateTime.Now,
                    CustomerName = txtCustomerName.Text.Trim(),
                    Status = "Новый"
                };

                AppData.Context.Orders.Add(newOrder);
                AppData.SaveAllData();
                LoadOrders();

                txtCustomerName.Text = "";
                txtOrderQuantity.Text = "1";

                UpdateStatus($"Новый заказ #{newOrder.Id} успешно добавлен");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}