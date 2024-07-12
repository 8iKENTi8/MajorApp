using MajorApp.Models;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            textBoxDescription.Text = _order.Description;
            textBoxPickupAddress.Text = _order.PickupAddress;
            textBoxDeliveryAddress.Text = _order.DeliveryAddress;
            textBoxComment.Text = _order.Comment;
            textBoxExecutor.Text = _order.Executor;
            textBoxWidth.Text = _order.Width.ToString();
            textBoxHeight.Text = _order.Height.ToString();
            textBoxDepth.Text = _order.Depth.ToString();
            textBoxWeight.Text = _order.Weight.ToString();

            // Установка текущего статуса заявки
            comboBoxStatus.SelectedItem = comboBoxStatus.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _order.Status);

            // Установка состояния полей
            SetFieldsState();
        }

        private void SetFieldsState()
        {
            bool isEditable = _order.Status == "Новая";

            textBoxDescription.IsEnabled = isEditable;
            textBoxPickupAddress.IsEnabled = isEditable;
            textBoxDeliveryAddress.IsEnabled = isEditable;
            textBoxExecutor.IsEnabled = isEditable;
            textBoxWidth.IsEnabled = isEditable;
            textBoxHeight.IsEnabled = isEditable;
            textBoxDepth.IsEnabled = isEditable;
            textBoxWeight.IsEnabled = isEditable;

            var readOnlyColor = new SolidColorBrush(Color.FromRgb(211, 211, 211)); // Light Gray
            var editableColor = new SolidColorBrush(Colors.White);

            textBoxDescription.Background = isEditable ? editableColor : readOnlyColor;
            textBoxPickupAddress.Background = isEditable ? editableColor : readOnlyColor;
            textBoxDeliveryAddress.Background = isEditable ? editableColor : readOnlyColor;
            textBoxExecutor.Background = isEditable ? editableColor : readOnlyColor;
            textBoxWidth.Background = isEditable ? editableColor : readOnlyColor;
            textBoxHeight.Background = isEditable ? editableColor : readOnlyColor;
            textBoxDepth.Background = isEditable ? editableColor : readOnlyColor;
            textBoxWeight.Background = isEditable ? editableColor : readOnlyColor;

            // Поле комментарий всегда редактируемо
            textBoxComment.IsEnabled = true;
            textBoxComment.Background = editableColor;
        }
        private async void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Обновление данных заказа с помощью API
            _order.Description = textBoxDescription.Text;
            _order.PickupAddress = textBoxPickupAddress.Text;
            _order.DeliveryAddress = textBoxDeliveryAddress.Text;
            _order.Comment = textBoxComment.Text;
            _order.Executor = textBoxExecutor.Text;
            _order.Width = double.TryParse(textBoxWidth.Text, out var width) ? width : _order.Width;
            _order.Height = double.TryParse(textBoxHeight.Text, out var height) ? height : _order.Height;
            _order.Depth = double.TryParse(textBoxDepth.Text, out var depth) ? depth : _order.Depth;
            _order.Weight = double.TryParse(textBoxWeight.Text, out var weight) ? weight : _order.Weight;

            // Обновление статуса заявки
            if (comboBoxStatus.SelectedItem is ComboBoxItem selectedStatus)
            {
                _order.Status = selectedStatus.Content.ToString();
            }

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

        private void ComboBoxStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Не позволяем менять статус обратно на "Новая", если он уже изменен
            if (_order.Status != "Новая" && ((ComboBoxItem)comboBoxStatus.SelectedItem).Content.ToString() == "Новая")
            {
                comboBoxStatus.SelectedItem = comboBoxStatus.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == _order.Status);
                MessageBox.Show("Статус заявки нельзя вернуть на 'Новая'");
            }
        }


    }
}