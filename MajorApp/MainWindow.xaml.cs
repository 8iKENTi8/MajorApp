using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using MajorApp.Models; 

namespace MajorApp
{
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void CreateOrder(object sender, RoutedEventArgs e)
        {
            var order = new Order
            {
                Description = textBoxDescription.Text,
                PickupAddress = textBoxPickupAddress.Text,
                DeliveryAddress = textBoxDeliveryAddress.Text,
                Comment = textBoxComment.Text,
                CreatedDate = datePickerCreatedDate.SelectedDate.GetValueOrDefault(),
                UpdatedDate = DateTime.Now,
                Executor = textBoxExecutor.Text,
                Width = double.Parse(textBoxWidth.Text),
                Height = double.Parse(textBoxHeight.Text),
                Depth = double.Parse(textBoxDepth.Text),
                Weight = double.Parse(textBoxWeight.Text)
            };

            var json = JsonSerializer.Serialize(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://localhost:5001/api/orders", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Order created successfully.");
                }
                else
                {
                    MessageBox.Show($"Failed to create order. Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request error: {ex.Message}");
            }
        }

        private void OpenOrdersWindow(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow();
            ordersWindow.Show();
            Hide();
        }

    }

}
