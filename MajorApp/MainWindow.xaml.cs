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
            // Проверка обязательных полей и корректности данных
            if (!IsValidInput())
            {
                MessageBox.Show("Пожалуйста, проверьте все поля на корректность. Все поля должны быть заполнены и иметь корректные значения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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

        private bool IsValidInput()
        {
            // Проверка даты создания
            if (datePickerCreatedDate.SelectedDate == null)
            {
                MessageBox.Show("Дата создания должна быть выбрана.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка обязательных текстовых полей
            if (string.IsNullOrWhiteSpace(textBoxDescription.Text) ||
                string.IsNullOrWhiteSpace(textBoxPickupAddress.Text) ||
                string.IsNullOrWhiteSpace(textBoxDeliveryAddress.Text) ||
                string.IsNullOrWhiteSpace(textBoxExecutor.Text))
            {
                MessageBox.Show("Описание, адрес получения, адрес доставки и исполнитель должны быть заполнены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Проверка числовых полей
            if (!double.TryParse(textBoxWidth.Text, out double width) || width <= 0 ||
                !double.TryParse(textBoxHeight.Text, out double height) || height <= 0 ||
                !double.TryParse(textBoxDepth.Text, out double depth) || depth <= 0 ||
                !double.TryParse(textBoxWeight.Text, out double weight) || weight <= 0)
            {
                MessageBox.Show("Ширина, высота, глубина и вес должны быть положительными числами.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void NumericOnly_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Метод для проверки ввода числовых данных
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string text)
        {
            // Метод для проверки корректности текста
            return double.TryParse(text, out _);
        }

        private void OpenOrdersWindow(object sender, RoutedEventArgs e)
        {
            OrdersWindow ordersWindow = new OrdersWindow();
            ordersWindow.Show();
            Hide();
        }
    }
}
