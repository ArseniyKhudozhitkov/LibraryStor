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
    public partial class BooksPage : Page
    {
        private User _currentUser;
        private List<Book> _allBooks;
        private bool _isInitialized = false;

        public BooksPage(User user)
        {
            InitializeComponent();
            _currentUser = user;
            Loaded += BooksPage_Loaded; 
        }

        private void BooksPage_Loaded(object sender, RoutedEventArgs e)
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

            LoadBooks();
            InitializeFilters();
        }

        private void LoadBooks()
        {
            _allBooks = AppData.Context.Books;
            booksGrid.ItemsSource = _allBooks;
        }

        private void InitializeFilters()
        {
            if (cmbGenreFilter == null || cmbSort == null || _allBooks == null)
                return;

            var genres = _allBooks.Select(b => b.Genre).Distinct().ToList();
            cmbGenreFilter.Items.Clear();
            cmbGenreFilter.Items.Add(new ComboBoxItem { Content = "Все жанры", IsSelected = true });
            foreach (var genre in genres)
            {
                cmbGenreFilter.Items.Add(new ComboBoxItem { Content = genre });
            }

            cmbSort.Items.Clear();
            cmbSort.Items.Add(new ComboBoxItem { Content = "По названию", IsSelected = true });
            cmbSort.Items.Add(new ComboBoxItem { Content = "По автору" });
            cmbSort.Items.Add(new ComboBoxItem { Content = "По цене (возр.)" });
            cmbSort.Items.Add(new ComboBoxItem { Content = "По цене (убыв.)" });
        }

        private void ApplyFilters()
        {
            if (_allBooks == null || booksGrid == null) return;

            var filteredBooks = _allBooks.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(txtSearch?.Text))
            {
                filteredBooks = filteredBooks.Where(b =>
                    b.Title.ToLower().Contains(txtSearch.Text.ToLower()) ||
                    b.Author.ToLower().Contains(txtSearch.Text.ToLower()));
            }

            if (cmbGenreFilter?.SelectedItem is ComboBoxItem genreItem && genreItem.Content?.ToString() != "Все жанры")
            {
                filteredBooks = filteredBooks.Where(b => b.Genre == genreItem.Content.ToString());
            }

            if (cmbSort?.SelectedItem is ComboBoxItem sortItem)
            {
                switch (sortItem.Content?.ToString())
                {
                    case "По автору":
                        filteredBooks = filteredBooks.OrderBy(b => b.Author);
                        break;
                    case "По цене (возр.)":
                        filteredBooks = filteredBooks.OrderBy(b => b.Price);
                        break;
                    case "По цене (убыв.)":
                        filteredBooks = filteredBooks.OrderByDescending(b => b.Price);
                        break;
                    default: 
                        filteredBooks = filteredBooks.OrderBy(b => b.Title);
                        break;
                }
            }

            booksGrid.ItemsSource = filteredBooks.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInitialized)
                ApplyFilters();
        }

        private void cmbGenreFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
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