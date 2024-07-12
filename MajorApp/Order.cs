using System;

namespace MajorApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Новая"; // Значение по умолчаниюю
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
