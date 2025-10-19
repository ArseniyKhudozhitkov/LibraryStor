using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;

namespace Library.Data
{
    public class DatabaseContext
    {
        private const string DATA_FILE = "library_data.json";

        public List<User> Users { get; set; }
        public List<Book> Books { get; set; }
        public List<Order> Orders { get; set; }

        public DatabaseContext()
        {
            InitializeData();
        }

        private void InitializeData()
        {
            Users = new List<User>
            {
                new User { Id = 1, Username = "client", Password = "client123", Role = "Client" },
                new User { Id = 2, Username = "manager", Password = "manager123", Role = "Manager" },
                new User { Id = 3, Username = "admin", Password = "admin123", Role = "Admin" }
            };

            Books = new List<Book>
            {
                new Book { Id = 1, Title = "Лавка запретных книг", Author = "Леви Марк", Genre = "Левиада", Price = 1000, Quantity = 10, Discount = 0 },
                new Book { Id = 2, Title = "Сто лет одиночества", Author = "Габриэль Гарсиа Маркес", Genre = "Проза", Price = 1450, Quantity = 8, Discount = 10 },
                new Book { Id = 3, Title = "Тихий Дон", Author = "Михаил Шолохов", Genre = "Роман", Price = 850, Quantity = 5, Discount = 20 },
                new Book { Id = 4, Title = "1984", Author = "Джордж Оруэлл", Genre = "Антиутопия", Price = 1400, Quantity = 12, Discount = 5 },
                new Book { Id = 5, Title = "Маленький принц", Author = "Антуан де Сент-Экзюпери", Genre = "Сказка", Price = 700, Quantity = 15, Discount = 15 }
            };

            Orders = new List<Order>
            {
                new Order { Id = 1, BookId = 1, Quantity = 2, OrderDate = System.DateTime.Now.AddDays(-5), CustomerName = "Художитков Арсений Константинович", Status = "Выполнен" },
                new Order { Id = 2, BookId = 4, Quantity = 1, OrderDate = System.DateTime.Now.AddDays(-3), CustomerName = "Иванов Пётор Михалович", Status = "В обработке" },
                new Order { Id = 3, BookId = 2, Quantity = 3, OrderDate = System.DateTime.Now.AddDays(-4), CustomerName = "Чуганина Мария Сергеевна", Status = "Доставляется" }
            };

            LoadData();
        }

        private void LoadData()
        {
            if (File.Exists(DATA_FILE))
            {
                try
                {
                    string json = File.ReadAllText(DATA_FILE);
                    var data = JsonConvert.DeserializeObject<StoreData>(json);

                    if (data != null)
                    {
                        Users = data.Users ?? Users;
                        Books = data.Books ?? Books;
                        Orders = data.Orders ?? Orders;
                    }
                }
                catch
                {

                }
            }
        }

        public void SaveData()
        {
            try
            {
                var data = new StoreData
                {
                    Users = Users,
                    Books = Books,
                    Orders = Orders
                };

                string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(DATA_FILE, json);
            }
            catch
            {
                
            }
        }

        public User Authenticate(string username, string password)
        {
            return Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }


        public bool RegisterUser(string username, string password, string email, string fullName)
        {

            if (Users.Any(u => u.Username == username))
            {
                return false;
            }


            if (!string.IsNullOrEmpty(email) && Users.Any(u => u.Email == email))
            {
                return false;
            }

            var newUser = new User
            {
                Id = Users.Count > 0 ? Users.Max(u => u.Id) + 1 : 1,
                Username = username,
                Password = password,
                Email = email,
                FullName = fullName,
                Role = "Client"
            };

            Users.Add(newUser);
            SaveData(); 
            return true;
        }


        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return true; 

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

    
        public bool ValidatePassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        private class StoreData
        {
            public List<User> Users { get; set; }
            public List<Book> Books { get; set; }
            public List<Order> Orders { get; set; }
        }
    }
}
