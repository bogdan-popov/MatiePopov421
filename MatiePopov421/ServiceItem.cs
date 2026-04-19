using Avalonia.Media.Imaging;
using System;

namespace MatiePopov421
{
    public class ServiceItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImagePath { get; set; } = "";
        public Bitmap? Photo { get; set; }
        public int CollectionId { get; set; }
        public int TypeId { get; set; }
        public decimal Price { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedFormatted => LastModifiedAt.ToString("dd.MM.yyyy HH:mm");
        public string PriceFormatted => Price > 0 ? $"{Price:N0} ₽" : "Цена не указана";
    }
}
