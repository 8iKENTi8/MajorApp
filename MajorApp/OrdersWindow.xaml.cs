using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MajorApp.Models;

namespace MajorApp
{
    public partial class OrdersWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private List<Order> _orders;  // Храним исходный список заказов

        public OrdersWindow()
        {
            InitializeComponent();
            LoadOrders();  // Загрузка данных при создании окна
        }

        // Универсальный метод для загрузки и обновления данных в DataGrid
        private async void LoadOrders()
        {
            try
            {
                var response = await client.GetAsync("https://localhost:5001/api/orders");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    _orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    dataGridOrders.ItemsSource = _orders;  // Устанавливаем список заказов в DataGrid
                }
                else
                {
                    MessageBox.Show($"Failed to load orders. Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request error: {ex.Message}");
            }
        }

        // Метод для создания новой заявки
        private void CreateNewOrder(object sender, RoutedEventArgs e)
        {
            var createOrderWindow = new MainWindow();
            createOrderWindow.Show();
            Hide();
        }

        // Метод для удаления заявки
        private async void DeleteOrder(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.DataContext as Order;

            if (order == null)
                return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ {order.Id}?", "Удалить заявку", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var response = await client.DeleteAsync($"https://localhost:5001/api/orders/{order.Id}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Order deleted successfully.");
                        LoadOrders();  // Обновление данных после удаления заявки
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete order. Status Code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Request error: {ex.Message}");
                }
            }
        }

        // Метод для поиска заявок по введенному тексту
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchTextBox.Text.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                dataGridOrders.ItemsSource = _orders;  // Если поле поиска пустое, загружаем все заявки
                return;
            }

            var filteredOrders = _orders.Where(o =>
                (o.Status?.ToLowerInvariant().Contains(searchText) == true) ||
                (o.Description?.ToLowerInvariant().Contains(searchText) == true) ||
                (o.PickupAddress?.ToLowerInvariant().Contains(searchText) == true) ||
                (o.DeliveryAddress?.ToLowerInvariant().Contains(searchText) == true) ||
                (o.Executor?.ToLowerInvariant().Contains(searchText) == true) ||
                (o.Comment?.ToLowerInvariant().Contains(searchText) == true) ||
                (o.CreatedDate.ToString("yyyy-MM-dd").ToLowerInvariant().Contains(searchText)) ||  // Преобразуем DateTime в строку для поиска
                (o.UpdatedDate.ToString("yyyy-MM-dd").ToLowerInvariant().Contains(searchText)) ||  // Преобразуем DateTime в строку для поиска
                (o.Width.ToString().ToLowerInvariant().Contains(searchText)) ||
                (o.Height.ToString().ToLowerInvariant().Contains(searchText)) ||
                (o.Depth.ToString().ToLowerInvariant().Contains(searchText)) ||
                (o.Weight.ToString().ToLowerInvariant().Contains(searchText))
            ).ToList();

            dataGridOrders.ItemsSource = new ObservableCollection<Order>(filteredOrders);  // Преобразуем отфильтрованные заказы в ObservableCollection
        }

    }
}
