namespace MyApi.Controllers.Models
{
    internal class Product
    {
        public long? ID { get; set; }
        public string? SKU { get; set; }
        public string? name { get; set; }
        public long? EAN { get; set; }
        public string? producer_name { get; set; }
        public string? category { get; set; }
        public bool? is_wire { get; set; }
        public string? shipping { get; set; }
        public bool? available { get; set; }
        public bool? is_vendor { get; set; }
        public string? default_image { get; set; }
    }
}
