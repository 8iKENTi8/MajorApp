using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

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
    }

    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string PickupAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Executor { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Depth { get; set; }
        public double Weight { get; set; }
    }
}
