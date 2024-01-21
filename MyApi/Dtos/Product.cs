using MyApi.Controllers.Models;

namespace MyApi.Dtos
{
    public class Product
    {
        public string? Name { get; set; }
        public long? EAN { get; set; }
        public string? Producer_name { get; set; }
        public string? Category { get; set; }
        public string? Default_image { get; set; }


        public string? Unit { get; set; }
        public double? Qty { get; set; }
        public decimal? Shipping_cost { get; set; }


        public decimal? Price { get; set; }
    }
}