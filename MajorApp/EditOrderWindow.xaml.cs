using MajorApp.Models;
using System.Net.Http;
using System.Text.Json;
using System.Windows;

namespace MajorApp
{
    /// <summary>
    /// Interaction logic for EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private Order _order;

        public EditOrderWindow(Order order)
        {
            InitializeComponent();
            _order = order;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            // Заполнение полей данными из выбранного заказа
            datePickerCreatedDate.SelectedDate = _order.CreatedDate;
            textBoxDescription.Text = _order.Description;
            textBoxPickupAddress.Text = _order.PickupAddress;
            textBoxDeliveryAddress.Text = _order.DeliveryAddress;
            textBoxComment.Text = _order.Comment;
            textBoxExecutor.Text = _order.Executor;
            textBoxWidth.Text = _order.Width.ToString();
            textBoxHeight.Text = _order.Height.ToString();
            textBoxDepth.Text = _order.Depth.ToString();
            textBoxWeight.Text = _order.Weight.ToString();
        }
        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Обновление данных заказа с помощью API
            _order.CreatedDate = datePickerCreatedDate.SelectedDate ?? _order.CreatedDate;
            _order.Description = textBoxDescription.Text;
            _order.PickupAddress = textBoxPickupAddress.Text;
            _order.DeliveryAddress = textBoxDeliveryAddress.Text;
            _order.Comment = textBoxComment.Text;
            _order.Executor = textBoxExecutor.Text;
            _order.Width = double.TryParse(textBoxWidth.Text, out var width) ? width : _order.Width;
            _order.Height = double.TryParse(textBoxHeight.Text, out var height) ? height : _order.Height;
            _order.Depth = double.TryParse(textBoxDepth.Text, out var depth) ? depth : _order.Depth;
            _order.Weight = double.TryParse(textBoxWeight.Text, out var weight) ? weight : _order.Weight;

            try
            {
                var json = JsonSerializer.Serialize(_order);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"https://localhost:5001/api/orders/{_order.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Изменения сохранены");
                    Close();
                }
                else
                {
                    MessageBox.Show($"Failed to update order. Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request error: {ex.Message}");
            }
        }
    }
}