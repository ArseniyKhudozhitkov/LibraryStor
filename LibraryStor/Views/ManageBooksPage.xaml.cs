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
    public partial class ManageBooksPage : Page
    {
        private int _currentBookId = -1;

        public ManageBooksPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBooks();
            ClearForm();
            UpdateStatus("Готов к работе");
        }

        private void LoadBooks()
        {
            booksGrid.ItemsSource = null;
            booksGrid.ItemsSource = AppData.Context.Books.ToList();
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtAuthor.Text = "";
            txtGenre.Text = "";
            txtPrice.Text = "";
            txtQuantity.Text = "";
            txtDiscount.Text = "0";
            _currentBookId = -1;
            btnSave.IsEnabled = false;
        }

        private void UpdateStatus(string message)
        {
            statusText.Text = message;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Введите автора книги", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Введите корректное количество", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!decimal.TryParse(txtDiscount.Text, out decimal discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Введите корректную скидку (0-100%)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                int newId = AppData.Context.Books.Count > 0 ? AppData.Context.Books.Max(b => b.Id) + 1 : 1;

                var newBook = new Book
                {
                    Id = newId,
                    Title = txtTitle.Text.Trim(),
                    Author = txtAuthor.Text.Trim(),
                    Genre = txtGenre.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text),
                    Quantity = int.Parse(txtQuantity.Text),
                    Discount = decimal.Parse(txtDiscount.Text)
                };

                AppData.Context.Books.Add(newBook);
                AppData.SaveAllData();
                LoadBooks();
                ClearForm();
                UpdateStatus($"Книга '{newBook.Title}' успешно добавлена");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Book book)
            {
                txtTitle.Text = book.Title;
                txtAuthor.Text = book.Author;
                txtGenre.Text = book.Genre;
                txtPrice.Text = book.Price.ToString();
                txtQuantity.Text = book.Quantity.ToString();
                txtDiscount.Text = book.Discount.ToString();

                _currentBookId = book.Id;
                btnSave.IsEnabled = true;
                UpdateStatus($"Редактирование книги: {book.Title}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBookId == -1)
            {
                MessageBox.Show("Не выбрана книга для редактирования", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                var book = AppData.Context.Books.FirstOrDefault(b => b.Id == _currentBookId);
                if (book != null)
                {
                    book.Title = txtTitle.Text.Trim();
                    book.Author = txtAuthor.Text.Trim();
                    book.Genre = txtGenre.Text.Trim();
                    book.Price = decimal.Parse(txtPrice.Text);
                    book.Quantity = int.Parse(txtQuantity.Text);
                    book.Discount = decimal.Parse(txtDiscount.Text);

                    AppData.SaveAllData();
                    LoadBooks();
                    ClearForm();
                    UpdateStatus($"Книга '{book.Title}' успешно обновлена");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is Book book)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить книгу '{book.Title}'?",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AppData.Context.Books.Remove(book);
                        AppData.SaveAllData();
                        LoadBooks();
                        if (_currentBookId == book.Id)
                        {
                            ClearForm();
                        }
                        UpdateStatus($"Книга '{book.Title}' успешно удалена");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            UpdateStatus("Редактирование отменено");
        }

        private void booksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (booksGrid.SelectedItem is Book book)
            {
                EditButton_Click(null, new RoutedEventArgs());
            }
        }
    }
}
