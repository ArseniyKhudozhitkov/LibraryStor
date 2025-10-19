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
    public partial class OrdersPage : Page
    {
        private User _currentUser;
        private List<OrderDisplay> _allOrders;
        private bool _isInitialized = false;

        public OrdersPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            Loaded += Page_Loaded;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
            {
                InitializePage();
                _isInitialized = true;
            }
        }

        private void InitializePage()
        {
            if (_currentUser.Role == "Guest" || _currentUser.Role == "Client")
            {
                controlPanel.Visibility = Visibility.Collapsed;
            }

            LoadOrders();
            InitializeFilters();
        }

        private void LoadOrders()
        {
            _allOrders = new List<OrderDisplay>();

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
                _allOrders.Add(orderDisplay);
            }

            ordersGrid.ItemsSource = _allOrders;
            UpdateNoOrdersMessage();
        }

        private void InitializeFilters()
        {
            if (cmbStatusFilter == null || cmbSort == null || _allOrders == null)
                return;

            var statuses = _allOrders.Select(o => o.Status).Distinct().ToList();
            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.Add(new ComboBoxItem { Content = "Все статусы", IsSelected = true });
            foreach (var status in statuses)
            {
                cmbStatusFilter.Items.Add(new ComboBoxItem { Content = status });
            }

            cmbSort.Items.Clear();
            cmbSort.Items.Add(new ComboBoxItem { Content = "По дате (новые)", IsSelected = true });
            cmbSort.Items.Add(new ComboBoxItem { Content = "По дате (старые)" });
            cmbSort.Items.Add(new ComboBoxItem { Content = "По клиенту" });
        }

        private void ApplyFilters()
        {
            if (_allOrders == null || ordersGrid == null) return;

            var filteredOrders = _allOrders.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtSearch?.Text))
            {
                filteredOrders = filteredOrders.Where(order =>
                    order.CustomerName.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    order.BookTitle.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    order.Status.ToLower().Contains(txtSearch.Text.ToLower()));
            }

            if (cmbStatusFilter?.SelectedItem is ComboBoxItem statusItem && statusItem.Content?.ToString() != "Все статусы")
            {
                filteredOrders = filteredOrders.Where(order => order.Status == statusItem.Content.ToString());
            }

            if (cmbSort?.SelectedItem is ComboBoxItem sortItem)
            {
                switch (sortItem.Content?.ToString())
                {
                    case "По дате (старые)":
                        filteredOrders = filteredOrders.OrderBy(order => order.OrderDate);
                        break;
                    case "По клиенту":
                        filteredOrders = filteredOrders.OrderBy(order => order.CustomerName);
                        break;
                    default: 
                        filteredOrders = filteredOrders.OrderByDescending(order => order.OrderDate);
                        break;
                }
            }

            ordersGrid.ItemsSource = filteredOrders.ToList();
            UpdateNoOrdersMessage();
        }

        private void UpdateNoOrdersMessage()
        {
            if (ordersGrid.ItemsSource == null || ((System.Collections.IList)ordersGrid.ItemsSource).Count == 0)
            {
                txtNoOrders.Visibility = Visibility.Visible;
                ordersGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtNoOrders.Visibility = Visibility.Collapsed;
                ordersGrid.Visibility = Visibility.Visible;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInitialized)
                ApplyFilters();
        }

        private void cmbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitialized)
                ApplyFilters();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitialized)
                ApplyFilters();
        }
    }
}